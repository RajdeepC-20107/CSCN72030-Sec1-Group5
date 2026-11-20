namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for creating a new HelloDevice.
/// </summary>
public sealed record CreateHelloDeviceRequest(
    string? RequestId,
    CreateHelloDevicePayload? Payload
) : CommandRequest(RequestId);

public sealed record CreateHelloDevicePayload(string? Message);
