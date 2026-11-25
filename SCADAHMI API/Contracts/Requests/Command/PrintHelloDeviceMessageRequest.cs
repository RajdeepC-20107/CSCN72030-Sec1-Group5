namespace SCADAHMI_API.Contracts.Requests.Command;

/// <summary>
/// Request envelope for printing the message of all HelloDevices.
/// </summary>
public sealed record PrintHelloDeviceMessageRequest(
    string? RequestId
) : ApiRequest(RequestId);
