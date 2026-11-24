namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

/// <summary>
/// Device creation payload for Lights.
/// </summary>
/// <param name="MinimumBrightness"></param>
/// <param name="MaximumBrightness"></param>
/// <param name="Red"></param>
/// <param name="Green"></param>
/// <param name="Blue"></param>
public sealed record CreateLightsPayload(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDevicePayload;
