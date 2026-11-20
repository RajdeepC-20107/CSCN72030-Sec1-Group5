namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

/// <summary>
/// Request envelope for creating a new device.
/// </summary>
public sealed record CreateDeviceRequest(
    string? RequestId,
    CreateDevicePayload? Payload
) : CommandRequest(RequestId);
