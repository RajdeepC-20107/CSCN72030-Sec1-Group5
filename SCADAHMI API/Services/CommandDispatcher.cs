using System.Collections.Immutable;
using SCADAHMI_API.Contracts.Requests;
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
    public ApiResponse Dispatch(CommandRequest request) => request switch
    {
        SetHelloDeviceMessageRequest setMessage => 
            SetHelloDeviceMessage(setMessage),
        PrintHelloDeviceMessageRequest printMessages => 
            PrintHelloDeviceMessages(printMessages),
        _ => 
            new ApiResponse(
                Normalize(request.RequestId),
                false,
                Error: $"Unsupported command request: {request.GetType().Name}"
            )
    };

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

    private static string Normalize(string? requestId) =>
        string.IsNullOrWhiteSpace(requestId) ? Guid.NewGuid().ToString() : requestId;
}
