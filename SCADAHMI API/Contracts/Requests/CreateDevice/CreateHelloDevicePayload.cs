namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

/// <summary>
/// Device creation payload for HelloDevices.
/// </summary>
/// <param name="Message"></param>
public sealed record CreateHelloDevicePayload(
    string? Message
) : CreateDevicePayload;
