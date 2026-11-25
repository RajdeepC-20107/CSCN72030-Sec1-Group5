namespace SCADAHMI_API.Contracts.Requests.Command;

/// <summary>
/// Request envelope for renaming multiple devices.
/// </summary>
public sealed record RenameDeviceRequest(
    string? RequestId,
    RenameDevicePayload Payload
) : ApiRequest(RequestId);

/// <summary>
/// Payload for renaming devices by GUID.
/// </summary>
/// <param name="DeviceNames">Maps device GUIDs to their new names.</param>
public sealed record RenameDevicePayload(
    Dictionary<Guid, string> DeviceNames
);
