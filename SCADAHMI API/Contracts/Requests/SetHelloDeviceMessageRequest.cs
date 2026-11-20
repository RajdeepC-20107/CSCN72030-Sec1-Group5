namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for setting the message of all HelloDevices.
/// </summary>
public sealed record SetHelloDeviceMessageRequest(
    string? RequestId,
    SetHelloDeviceMessagePayload Payload
) : CommandRequest(RequestId);

public sealed record SetHelloDeviceMessagePayload(string Message);
