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

app.MapPost("/api/commands/set-message",
    (SetHelloDeviceMessageRequest request, CommandDispatcher dispatcher) =>
    {
        ApiResponse response = dispatcher.Dispatch(request);
        return response.Success
            ? Results.Ok(response)
            : Results.BadRequest(response);
    })
    .WithName("SetHelloDeviceMessage")
    .WithTags("Commands");

app.MapPost("/api/devices", async (
    CreateHelloDeviceRequest request,
    DeviceRegistry registry,
    DeviceStateNotifier notifier) =>
    {
        HelloDevice? device = HelloDeviceManager.CreateDevice(
            new CreateHelloDeviceArgs(request.Payload?.Message), registry);

        string requestId = string.IsNullOrWhiteSpace(request.RequestId)
            ? Guid.NewGuid().ToString()
            : request.RequestId;

        if (device is null)
        {
            return Results.BadRequest(new ApiResponse(
                requestId, false, Error: "The device could not be registered."));
        }

        await notifier.PublishAsync(registry, registry.Version);

        return Results.Ok(new ApiResponse(
            requestId,
            true,
            new DeviceStateDto(device.Guid, nameof(HelloDevice), device.Message)));
    })
    .WithName("CreateHelloDevice")
    .WithTags("Devices");

app.MapGet("/api/devices", (DeviceRegistry registry, int since = 0) =>
    {
        // A client that has never fetched a snapshot (since == 0) always
        // gets the full state; otherwise 304 means "nothing changed".
        if (since > 0 && since >= registry.Version)
        {
            return Results.StatusCode(StatusCodes.Status304NotModified);
        }

        return Results.Ok(DeviceSnapshotFactory.Create(registry, registry.Version));
    })
    .WithName("GetDevices")
    .WithTags("Devices");

// Bridge core command execution to SignalR clients.
DeviceStateNotifier notifier = app.Services.GetRequiredService<DeviceStateNotifier>();
CommandBus.CommandExecuted += (_, e) => _ = notifier.PublishAsync(e.Registry, e.Version);

app.Run();
