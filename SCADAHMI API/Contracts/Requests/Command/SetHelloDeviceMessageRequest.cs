namespace SCADAHMI_API.Contracts.Requests.Command;

/// <summary>
/// Request envelope for setting the message of multiple HelloDevices.
/// </summary>
public sealed record SetHelloDeviceMessageRequest(
    string? RequestId,
    SetHelloDeviceMessagePayload Payload
) : ApiRequest(RequestId);

/// <summary>
/// Payload for setting the messages of multiple HelloDevices.
/// </summary>
/// <param name="Messages">Maps device GUIDs to their new messages.</param>
public sealed record SetHelloDeviceMessagePayload(
    Dictionary<Guid, string> Messages
);
