namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for renaming a device.
/// </summary>
public sealed record RenameDeviceRequest(
    string? RequestId,
    RenameDevicePayload Payload
) : CommandRequest(RequestId);

public sealed record RenameDevicePayload(
    Dictionary<Guid, string> DeviceNames
);
