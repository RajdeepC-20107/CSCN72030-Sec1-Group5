namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

/// <summary>
/// Remove device payload for HelloDevices.
/// </summary>
/// <param name="Guid"></param>
public sealed record RemoveHelloDevicePayload(
    Guid Guid
) : RemoveDevicePayload(Guid);
