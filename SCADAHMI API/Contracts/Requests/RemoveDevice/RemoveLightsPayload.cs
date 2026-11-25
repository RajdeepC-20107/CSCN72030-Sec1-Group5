namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

/// <summary>
/// Remove device payload for Lights.
/// </summary>
/// <param name="Guid"></param>
public sealed record RemoveLightsPayload(
    Guid Guid
) : RemoveDevicePayload(Guid);
