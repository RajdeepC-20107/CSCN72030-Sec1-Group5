namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Request envelope for setting the RGB of specified lights.
/// </summary>
public sealed record SetLightsRGBRequest(
    string? RequestId,
    SetLightsRGBPayload Payload
) : CommandRequest(RequestId);

public sealed record SetLightsRGBPayload(
    HashSet<Guid> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
);