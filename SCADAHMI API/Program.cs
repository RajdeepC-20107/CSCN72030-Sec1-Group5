using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Contracts.Requests;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_API.Data;
using SCADAHMI_API.Hubs;
using SCADAHMI_API.Services;
using SCADAHMI_Project;

var builder = WebApplication.CreateBuilder(args);

// API Controllers
builder.Services.AddControllers();

// JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// SignalR
builder.Services.AddSignalR();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SCADA HMI API",
        Version = "v1"
    });
});

// SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=absolute-cinema.db"
    )
);

// Local clients (browser, Android, CLI) need CORS during development.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("LocalClients", policy =>
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
    });
}

// Shared domain state and application services
builder.Services.AddSingleton<DeviceRegistry>(DeviceRegistry.Instance);
builder.Services.AddSingleton<CommandDispatcher>();
builder.Services.AddSingleton<DeviceStateNotifier>();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseCors("LocalClients");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SCADA HMI API v1");
        options.RoutePrefix = "swagger";
    });
}

// Plain HTTP is fine for a local class project; keep redirect in production.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Serve the local test client from wwwroot.
app.UseDefaultFiles();   // maps "/" to "/index.html"
app.UseStaticFiles();

app.MapHub<ScadaHub>("/hub");

// Any registry revision change is pushed to SignalR clients, no matter
// whether it came from a command, a manager, or a subscribe/unsubscribe.
DeviceStateNotifier notifier = app.Services.GetRequiredService<DeviceStateNotifier>();
DeviceRegistry deviceRegistry = app.Services.GetRequiredService<DeviceRegistry>();
deviceRegistry.Changed += (_, version) => _ = notifier.PublishAsync(deviceRegistry, version);

app.MapControllers();

app.Run();
