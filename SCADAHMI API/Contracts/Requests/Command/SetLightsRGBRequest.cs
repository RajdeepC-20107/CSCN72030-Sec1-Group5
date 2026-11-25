namespace SCADAHMI_API.Contracts.Requests.Command;

/// <summary>
/// Request envelope for setting the RGB of specified lights.
/// </summary>
public sealed record SetLightsRGBRequest(
    string? RequestId,
    SetLightsRGBPayload Payload
) : ApiRequest(RequestId);

/// <summary>
/// Payload for setting the RGB of multiple Lights. Only the channels supplied
/// are changed.
/// </summary>
/// <param name="Lights">The GUIDs of the Lights to change.</param>
/// <param name="Red"></param>
/// <param name="Green"></param>
/// <param name="Blue"></param>
public sealed record SetLightsRGBPayload(
    HashSet<Guid> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
);