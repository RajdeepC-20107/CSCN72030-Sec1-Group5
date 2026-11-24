using SCADAHMI_API.Contracts.Requests;
using SCADAHMI_API.Contracts.Requests.Command;
using SCADAHMI_API.Contracts.Responses;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Translates transport envelopes into domain commands. Knows nothing
/// about HTTP or SignalR, so it can be reused by any client adapter.
/// </summary>
public sealed class CommandDispatcher(DeviceRegistry registry)
{
    // TODO: Add ability to dispatch rename commands

    /// <summary>
    /// The method used to dispatch existing commands.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>An API response concerning the success or failure of a command dispatch.</returns>
    public ApiResponse Dispatch(ApiRequest request) => request switch
    {
        RenameDeviceRequest renameDevice =>
            RenameDevice(renameDevice),
        SetHelloDeviceMessageRequest setMessage => 
            SetHelloDeviceMessage(setMessage),
        PrintHelloDeviceMessageRequest printMessages => 
            PrintHelloDeviceMessages(printMessages),
        SetLightsRGBRequest lightsRGB => 
            SetLightsRGB(lightsRGB),
        _ => 
            new ApiResponse(
                Normalize(request.RequestId),
                false,
                Error: $"Unsupported command request: {request.GetType().Name}"
            )
    };

    private ApiResponse RenameDevice(RenameDeviceRequest request)
    {
        string requestId = Normalize(request.RequestId);

        if (request.Payload?.DeviceNames is null || request.Payload.DeviceNames.Count == 0)
        {
            return new ApiResponse(requestId, false, Error: "payload and/or messages must not be empty.");
        }

        try
        {
            Dictionary<Device, string> devices = (
                from d in registry.GetDevices()
                where request.Payload.DeviceNames.TryGetValue(d.Guid, out string? value)
                select (d, request.Payload.DeviceNames[d.Guid])
            ).ToDictionary();


            IDeviceManager<Device, CreateDeviceArgs>.RenameDevice(
                new RenameDeviceCommandArgs(devices),
                registry
            );

            return new ApiResponse(requestId, true);
        }
        catch (Exception ex)
        {
            return new ApiResponse(requestId, false, Error: ex.Message);
        }
    }

    /// <summary>
    /// Dispatch the SetHelloDeviceCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>An API response concerning the success or failure of the command dispatch.</returns>
    private ApiResponse SetHelloDeviceMessage(SetHelloDeviceMessageRequest request)
    {
        string requestId = Normalize(request.RequestId);

        if (request.Payload?.Messages is null || request.Payload.Messages.Count == 0)
        {
            return new ApiResponse(requestId, false, Error: "payload and/or messages must not be empty.");
        }

        try
        {
            IReadOnlyList<(HelloDevice, string)> devices = (
                from d in registry.GetDevicesOfType<HelloDevice>()
                where request.Payload.Messages.TryGetValue(d.Guid, out string? value)
                select (d, request.Payload.Messages[d.Guid])
            ).ToList();


            HelloDeviceManager.SetHelloDeviceMessages(
                devices.ToList(),
                registry
            );

            return new ApiResponse(requestId, true);
        }
        catch (Exception ex)
        {
            return new ApiResponse(requestId, false, Error: ex.Message);
        }
    }

    /// <summary>
    /// Dispatches the PrintHelloDeviceCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>An API response concerning the success or failure of the command dispatch.</returns>
    private ApiResponse PrintHelloDeviceMessages(PrintHelloDeviceMessageRequest request)
    {
        string requestId = Normalize(request.RequestId);

        try
        {
            HelloDeviceManager.PrintHelloDeviceMessages(
                registry
            );

            return new ApiResponse(requestId, true);
        }
        catch (Exception ex)
        {
            return new ApiResponse(requestId, false, Error: ex.Message);
        }
    }

    /// <summary>
    /// Dispatches the SetLightsRGBCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>An API response concerning the success or failure of the command dispatch.</returns>
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

    /// <summary>
    /// Used to create a requestId if one is not already made.
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    private static string Normalize(string? requestId) =>
        string.IsNullOrWhiteSpace(requestId) ? Guid.NewGuid().ToString() : requestId;
}
