namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for setting a device to be powered on or off.
/// </summary>
public sealed record SetDevicePowerRequest(
    string? RequestId,
    SetDevicePowerPayload Payload
) : CommandRequest(RequestId);

public sealed record SetDevicePowerPayload(
    Dictionary<Guid, bool> DevicePowerStates
);
