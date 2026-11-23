namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for setting the message of all HelloDevices.
/// </summary>
public sealed record PrintHelloDeviceMessageRequest(
    string? RequestId
) : CommandRequest(RequestId);
