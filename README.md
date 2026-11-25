# CSCN72030-SCADAHMI-Project

## Setup

### Requirements

.NET SDK 10.0 or higher is required.

### Required Packages

**SCADAHMI API Project**
```
dotnet add package Swashbuckle.AspNetCore --version 10.1.0
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.14
dotnet tool install --global dotnet-ef --version 10.0.14
```

### Run the webpage

From the terminal, with the entire project folder as the directory, run 
`dotnet run --project 'SCADAHMI API/SCADAHMI API.csproj'`, then open the
local host link.

## Process of adding a new device + commands

This is me writing a live documentation of how I added the lights device.

Some prior information: This process comes in three tiers, with the upper tier requiring to repeat the actions of all previous tiers. Additionally, affecting a higher tier will have a decent change of impacting the lower tiers. Starting with the highest tier:

### Tier 3: Adding a device

In `SCADAHMI Project/src/devices`, create a class of your new device, inheriting Device.cs. Make sure to give it a default name of `nameof({new device class})`. In my case, it would be `nameof(Lights)`. As well, I am adding what properties the lights will have.
```C#
public class Lights : Device
{
    public override string Name { get; set; } = nameof(Lights);
    
    public bool IsDimmed { get; set; } = false;
    private static readonly ushort MIN_BRIGHTNESS = 0;
    private static readonly ushort MAX_BRIGHTNESS = 100;
    public ushort MinimumBrightness
    {
        get;
        set { 
            field = Math.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS); 
            if(MaximumBrightness < field) {
                MaximumBrightness = field;
            }
        }
    } = 10;

    public ushort MaximumBrightness { 
        get; set
        {
            field = Math.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
            if(MinimumBrightness > field) {
                MinimumBrightness = field;
            }
        } 
    } = 100;

    public byte Red { get; set; } = 255;
    public byte Green { get; set; } = 255;
    public byte Blue { get; set; } = 255;
}
```

In `SCADAHMI Project/src/deviceManagers`, create a new device manager class for the newly added device implementing IDeviceManager, so `LightsManager` in my case. Additionally, a DeviceManager class will have creation arguments they can use if necessary, which you define under `SCADAHMI Project/src/deviceManagers/createDeviceArgs`.

**Device managers are strictly for subscribing/unsubscribing devices and publishing commands. In no case should they actually affect a device. Additionally, device managers are meant to contain only static methods, so it is recommended you make the constructor private to prevent mistakes.**
```C#
public record CreateLightsArgs(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDeviceArgs;
```
```C#
public class LightsManager : IDeviceManager<Lights, CreateLightsArgs>
{
    private LightsManager() {}

    public static Lights? CreateDevice(CreateLightsArgs args, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        Lights device = new();
        device.Red = args.Red ?? device.Red;
        device.Green = args.Green ?? device.Green;
        device.Blue = args.Blue ?? device.Blue;
        device.MinimumBrightness = args.MinimumBrightness ?? device.MinimumBrightness;
        device.MaximumBrightness = args.MaximumBrightness ?? device.MaximumBrightness;
        bool success = registry.Subscribe(device);
        return (success) ? device : null;
    }

    public static Lights? GetDevice(Guid guid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.GetDeviceOfType<Lights>(guid);
    }

    public static bool RemoveDevice(Lights device, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.Unsubscribe(device);
    }

    public static bool RemoveDevice(Guid deviceGuid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        Lights? device = GetDevice(deviceGuid, registry);
        if(device is null)
        {
            return false;
        }

        return registry.Unsubscribe(device);
    }
}
```
Now we move to the ASP.NET Web API project! Firstly, a DeviceStateDTO has to be made for the newly created device, under `SCADAHMI API/Contracts/DTOs`, inheriting DeviceStateDTO. As well, the file `SCADAHMI API/Contracts/DTOs/DeviceStateDTO.cs` needs to know about the newly created DTO's existence to use it for json polymorphism. These strings need to be in camel case, not pascal or snake or any other form. Also-also, you need to go to `SCADAHMI API/Services/DeviceStateDtoFactory.cs` and add to the Create switch the newly made DTO along with its associated device.

*Note: because these hard-coded strings appear in multiple places, there may be a static class added later with all these strings as readonly constants.*
```C#
public record LightsStateDto(
    Guid Guid,
    string Name,
    bool IsDimmed,
    ushort MinimumBrightness,
    ushort MaximumBrightness,
    byte Red,
    byte Green,
    byte Blue
) : DeviceStateDto(Guid, Name)
{
    //Adding a constructor here saves a huge headache
    public LightsStateDto(Lights device) :
        this(device.Guid, device.Name, device.IsDimmed,
        device.MinimumBrightness, device.MaximumBrightness,
        device.Red, device.Green, device.Blue) {}
}
```
```C#
[JsonDerivedType(typeof(HelloDeviceStateDto), "helloDevice")]
[JsonDerivedType(typeof(LightsStateDto), "lights")] //Newly added line
public abstract record DeviceStateDto(
    Guid Guid,
    string Name
);
```
```C#
public static class DeviceStateDtoFactory
{
    public static DeviceStateDto Create(Device device) => device switch
    {
        HelloDevice hello => new HelloDeviceStateDto(hello),
        Lights lights => new LightsStateDto(lights), //Newly added line
        _ => 
            throw new NotSupportedException($"No wire type for {device.GetType().Name}.")
    };
}
```
Next, you must go under `SCADAHMI API/Contracts/Requests`. Two folders are relevant: `CreateDevice` and `RemoveDevice`. Here, you must create a device-specific payload for device creation and removal, as well as add the json polymorphism capability to the base payload classes.
```C#
public sealed record CreateLightsPayload(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDevicePayload;
```
```C#
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CreateHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(CreateLightsPayload), "lights")] //Newly added line
public abstract record CreateDevicePayload;
```
```C#
public sealed record RemoveLightsPayload(
    Guid Guid
) : RemoveDevicePayload(Guid);
```
```C#
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RemoveHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(RemoveLightsPayload), "lights")] //Newly added line
public abstract record RemoveDevicePayload(
    Guid Guid
);
```
Finally, you must modify `SCADAHMI API/Controllers/DevicesController.cs`. In that class, you'll find the Create and Remove methods under an HttpPost associated to each of them. In both statements, there is a switch. You must modify that switch to be able to use your payload and manager.
*Yes, I know that code in which you need to mess with the method itself is one of the golden terrible designs, so lets just not be stupid about it and not relive the cautionary tale.*
```C#
[HttpPost("create")]
public IActionResult Create(CreateDeviceRequest request)
{
    string requestId = string.IsNullOrWhiteSpace(request.RequestId)
        ? Guid.NewGuid().ToString()
        : request.RequestId;

    Device? device = request.Payload switch
    {
        CreateHelloDevicePayload hello => HelloDeviceManager.CreateDevice(
            new CreateHelloDeviceArgs(hello.Message), registry
        ),
        CreateLightsPayload lights => LightsManager.CreateDevice( //Newly added line
            new CreateLightsArgs(
                lights.MinimumBrightness, lights.MaximumBrightness,
                lights.Red, lights.Green, lights.Blue
            ), registry
        ),
        _ => null
    };

    if (device is null)
    {
        return BadRequest(new ApiResponse(
            requestId,
            false,
            Error: "Unsupported or invalid device payload."));
    }

    return Ok(new ApiResponse(
        requestId,
        true,
        DeviceStateDtoFactory.Create(device))
    );
}
```
```C#
[HttpPost("remove")]
public IActionResult Remove(RemoveDeviceRequest request)
{
    string requestId = string.IsNullOrWhiteSpace(request.RequestId)
        ? Guid.NewGuid().ToString()
        : request.RequestId;

    if (request.Payload is null)
    {
        return BadRequest(new ApiResponse(
            requestId, false, Error: "A removal payload is required."));
    }

    bool? removed = request.Payload switch
    {
        RemoveHelloDevicePayload hello => HelloDeviceManager.RemoveDevice(hello.Guid, registry),
        RemoveLightsPayload lights => LightsManager.RemoveDevice(lights.Guid, registry), //Newly added line
        _ => null
    };

    if (removed is null)
    {
        return BadRequest(new ApiResponse(
            requestId, 
            false, 
            Error: "Unsupported device payload type."
        ));
    }

    if (removed is false)
    {
        return NotFound(new ApiResponse(
            requestId,
            false,
            Error: $"No device of specified type with GUID '{request.Payload.Guid}' is registered."
        ));
    }

    return Ok(new ApiResponse(
        requestId,
        true
    ));
}
```
After all that hard work, you can finally modify your actual frontend code to create and remove the new type of device! That was 6 files created and 5 files modified.

But of course, that is only the first of three tiers.

### Tier 2: Adding commands for the device

The properties that each device have are affected by commands which are published to `SCADAHMI Project/src/controllers/CommandBus.cs`. As of writing this, the command bus being used is syncrhonous. When it will be changed to asynchronous? Who knows, if ever.

For the README.md here, I'll only focus on one command. There's going to be a ton of commands to add per device, making this the longest part.

The first step is to create your new command under `SCADAHMI Project/src/commands`. You can go an additional folder down to `commandArgs` to create command arguments to pass in with the command you're creating. As well, commands must return whether they actually successfully made a change or not. This is because the program uses versioning.
```C#
public record SetLightsRGBCommandArgs(
    List<Lights> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
) : CommandArgs;
```
```C#
public sealed class SetLightsRGBCommand
    : IDeviceCommand<SetLightsRGBCommandArgs>
{
    bool IDeviceCommand<SetLightsRGBCommandArgs>.Execute(
        DeviceRegistry deviceRegistry, 
        SetLightsRGBCommandArgs args
    ) {
        if(args.Red is null && args.Green is null && args.Blue is null)
        {
            return false;
        }

        var devices = deviceRegistry.GetDevicesOfType<Lights>()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(Lights device in args.Lights)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)) 
            {
                if(args.Red is not null && subscribed.Red != args.Red)
                {
                    subscribed.Red = (byte)args.Red;
                    changed = true;
                }
                if(args.Green is not null && subscribed.Green != args.Green)
                {
                    subscribed.Green = (byte)args.Green;
                    changed = true;
                }
                if(args.Blue is not null && subscribed.Blue != args.Blue)
                {
                    subscribed.Blue = (byte)args.Blue;
                    changed = true;
                }
            }
        }

        return changed;
    }
}
```
After the command has been created, you must reciprocate the command to its associated manager. Nothing stops commands from being published to from the API itself, but these managers are the last change to add any sort of logic that must run before and after a command is published, while simply being more organized. It is **highly** recommended that you use device managers to publish commands, not the API code itself.
```C#
//Newly added method to LightsManager.cs
public static void SetLightsRGB(
        List<Lights> lights, 
        byte? red, 
        byte? green, 
        byte? blue,
        DeviceRegistry? registry = null
    )
    {
        registry ??= DeviceRegistry.Instance;
        CommandBus.Publish(
            new SetLightsRGBCommand(),
            new SetLightsRGBCommandArgs(lights, red, green, blue),
            registry
        );
    }
```
And with that done, we can go to the API project. In `SCADAHMI API/Contracts/Requests/Command`, create a new request and payload (if any arguments were made) for the command created.
```C#
public sealed record SetLightsRGBRequest(
    string? RequestId,
    SetLightsRGBPayload Payload
) : ApiRequest(RequestId);

public sealed record SetLightsRGBPayload(
    HashSet<Guid> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
);
```
Afterwards, you need to change `SCADAHMI API/Controllers/CommandsController.cs` and `SCADAHMI API/Services/CommandDispatcher.cs`. In the commands controller, you add the HTTP Post endpoint for the command. In the command dispatcher, you make sure the payload being sent is safe to consume. It is expected that the command dispatcher uses the device manager its supposed to use.
```C#
public class CommandsController(CommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost("rename-device")]
    public IActionResult RenameDevice(RenameDeviceRequest request)
        => StandardDispatch(request);

    [HttpPost("set-message")]
    public IActionResult SetMessage(SetHelloDeviceMessageRequest request)
        => StandardDispatch(request);

    [HttpPost("print-messages")]
    public IActionResult PrintMessages(PrintHelloDeviceMessageRequest request)
        => StandardDispatch(request);

    [HttpPost("set-lights-rgb")] //New HTTP Endpoint + Function
    public IActionResult SetLightsRGB(SetLightsRGBRequest request)
        => StandardDispatch(request);

    private IActionResult StandardDispatch(ApiRequest request)
    {
        ApiResponse response = dispatcher.Dispatch(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
```
```C#
namespace SCADAHMI_API.Services;


public sealed class CommandDispatcher(DeviceRegistry registry)
{
    // TODO: Add ability to dispatch rename commands
    public ApiResponse Dispatch(ApiRequest request) => request switch
    {
        SetHelloDeviceMessageRequest setMessage => 
            SetHelloDeviceMessage(setMessage),
        PrintHelloDeviceMessageRequest printMessages => 
            PrintHelloDeviceMessages(printMessages),
        SetLightsRGBRequest lightsRGB => //Newly added line
            SetLightsRGB(lightsRGB),
        _ => 
            new ApiResponse(
                Normalize(request.RequestId),
                false,
                Error: $"Unsupported command request: {request.GetType().Name}"
            )
    };

    private ApiResponse SetHelloDeviceMessage(SetHelloDeviceMessageRequest request)
    {
        //...
    }

    private ApiResponse PrintHelloDeviceMessages(PrintHelloDeviceMessageRequest request)
    {
        //...
    }

    // Newly added function
    private ApiResponse SetLightsRGB(SetLightsRGBRequest request)
    {
        string requestId = Normalize(request.RequestId);

        if (request.Payload?.Lights is null || request.Payload.Lights.Count == 0)
        {
            return new ApiResponse(requestId, false, Error: "payload and/or guids must not be empty.");
        }

        try
        {
            IReadOnlyList<Lights> lights = (
                from d in registry.GetDevicesOfType<Lights>()
                join entry in request.Payload.Lights 
                    on d.Guid equals entry
                select d
            ).ToList();


            LightsManager.SetLightsRGB(
                lights.ToList(),
                request.Payload.Red,
                request.Payload.Green,
                request.Payload.Blue,
                registry
            );

            return new ApiResponse(requestId, true);
        }
        catch (Exception ex)
        {
            return new ApiResponse(requestId, false, Error: ex.Message);
        }
    }

    private static string Normalize(string? requestId) =>
        string.IsNullOrWhiteSpace(requestId) ? Guid.NewGuid().ToString() : requestId;
}

```
And that is it! There's way less files to jump through in comparison. with 3 new files added and 3 files modified. As stated though, this process has to be repeated for every command, so it ends up taking the most time (probably). It can definitely get mind numbing.

Now all that's left is to use this API

### Tier 1: The frontend

And this is where my knowledge no longer can help, because I was not responsible for writing the web frontend. I had OpenCode write it! And I've also had OpenCode write out the rest of the section.

Firstly, on whatever frontend you make, you will need to set up SignalR to receive server-to-client pushes. The server already maps `/hub`, pushes a snapshot to each client the moment it connects, and broadcasts a new one whenever the device registry version changes. After you have that subscription in place, you won't need to touch the connection itself again — but you will need to teach the renderer about any new device type you add, otherwise it just shows up as "Unsupported".

The client subscribes with `connection.on("devicesChanged", ...)`. The method is called `DevicesChanged` in C#, but SignalR lowercases the first letter of hub method names on the wire, so JavaScript has to listen for `devicesChanged`. That is separate from the JSON naming policy, which is what turns C# properties like `MinimumBrightness` into `minimumBrightness`.

Secondly, the API has been made to be frontend-agnostic, meaning that it can be switched from a website to Android without changing any of this C# backend code. One practical difference: the web client is served by the API itself, so it can use relative URLs like `/api/devices/create`. An Android app is a separate process and needs an absolute host, plus an `INTERNET` permission and cleartext traffic allowed if you stick to plain HTTP.

The web client itself is just two files under `SCADAHMI API/wwwroot`: `index.html` holds the cards, inputs and buttons, and `app.js` holds all the SignalR and fetch logic. Every button is a `fetch` POST funnelled through one `postJson` helper, and the live snapshot list is rebuilt from whatever snapshot arrives — whether that came from a push or from the conditional `GET /api/devices?since=N` poll (which returns `304 Not Modified` when nothing changed).

#### Passing JSON from the frontend to the backend

There is one envelope for every POST, so you never need to go digging through the C# request records just to work out what shape to send:

```json
{
  "requestId": "any-unique-string",
  "payload": { ... }
}
```

`requestId` is optional — the server generates one when it is blank — but sending it makes it much easier to match a request to its log entry. Only the `payload` changes per endpoint. Shown are the endpoints that exist as of this README.md being written:

| Endpoint | `payload` |
|---|---|
| `POST /api/devices/create` | `{ "$type": "helloDevice", "message": "..." }` or `{ "$type": "lights", "minimumBrightness": 10, "maximumBrightness": 100, "red": 255, "green": 255, "blue": 255 }` |
| `POST /api/devices/remove` | `{ "$type": "lights", "guid": "..." }` — use the removed device's own discriminator |
| `POST /api/commands/set-message` | `{ "messages": { "<guid>": "new text" } }` |
| `POST /api/commands/set-lights-rgb` | `{ "lights": ["<guid>"], "green": 10 }` — only the channels you supply change |
| `POST /api/commands/print-messages` | omit `payload` entirely |

There are copy-pasteable samples for all of these in `SCADAHMI API.http`.

The camelCase property names are just `System.Text.Json` doing its job on the PascalCase C# records, so `minimumBrightness` binds to `MinimumBrightness`. The exception is the discriminator: `$type` is a literal property name and its values (`"helloDevice"`, `"lights"`) are hard-coded strings, so those must be typed exactly. A capital `"Lights"` will not work even though a capital `Red` would bind fine. Anything left out or sent as `null` falls back to the device default on creation, and for RGB it means "leave this channel alone".

Here is the helper the web client uses. Copy it and each button only has to supply its payload:

```js
async function postJson(url, body, label) {
    const response = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ requestId: crypto.randomUUID(), ...body })
    });
    const result = await response.json();
    // result = { requestId, success, data, error }
}
```

`response.ok` tells you the HTTP status (200 on success, 400 or 404 on failure); `result.success` repeats it in the body and `result.error` carries the message you show the user.

#### Rendering the snapshot

`renderSnapshot` branches on `device.$type` — that is the whole trick. HelloDevices get a text input and a Set button; Lights get three number inputs for red, green and blue plus a Set RGB button that posts to `/api/commands/set-lights-rgb`. Each Lights row also has a small square that takes its colour from the pushed RGB values, which is the easiest way to see a SignalR push actually land.

Two things that are easy to get wrong: devices are addressed by `device.guid` (there is no `device.id`), and the Remove button must forward that device's own `$type`. Hardcoding `"helloDevice"` there works right up until the first time you try to remove a Lights device.

Finally, after any change to `app.js`, bump the cache-busting query in `index.html` (`app.js?v=5` as of writing this), otherwise the browser will happily keep serving the old script and you will spend a while wondering why nothing changed.