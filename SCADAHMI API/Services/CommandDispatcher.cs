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
    public ApiResponse Dispatch(CommandRequest request) => request switch
    {
        SetHelloDeviceMessageRequest setMessage => SetHelloDeviceMessage(setMessage),
        _ => new ApiResponse(
            Normalize(request.RequestId),
            false,
            Error: $"Unsupported command request: {request.GetType().Name}")
    };

    private ApiResponse SetHelloDeviceMessage(SetHelloDeviceMessageRequest request)
    {
        string requestId = Normalize(request.RequestId);

        if (string.IsNullOrWhiteSpace(request.Payload?.Message))
        {
            return new ApiResponse(requestId, false, Error: "payload.message must not be empty.");
        }

        try
        {
            CommandBus.Publish(
                new SetHelloDeviceCommand(),
                new SetHelloDeviceCommandArgs(request.Payload.Message),
                registry);

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
