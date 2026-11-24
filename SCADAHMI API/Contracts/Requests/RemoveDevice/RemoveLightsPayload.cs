namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

public sealed record RemoveLightsPayload(
    Guid Guid
) : RemoveDevicePayload(Guid);
