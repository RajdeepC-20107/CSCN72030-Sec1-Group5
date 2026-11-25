using SCADAHMI_API.Contracts.Requests.Command;

namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

/// <summary>
/// Request envelope for removing an existing device.
/// </summary>
public sealed record RemoveDeviceRequest(
    string? RequestId,
    RemoveDevicePayload? Payload
) : ApiRequest(RequestId);
