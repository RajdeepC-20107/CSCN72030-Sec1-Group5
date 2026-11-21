namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

public sealed record RemoveHelloDevicePayload(
    Guid Guid
) : RemoveDevicePayload(Guid);
