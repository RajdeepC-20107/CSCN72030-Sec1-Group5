namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

public sealed record CreateLightsPayload(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDevicePayload;
