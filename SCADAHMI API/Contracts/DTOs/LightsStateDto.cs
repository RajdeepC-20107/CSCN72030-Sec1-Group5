using SCADAHMI_Project;

namespace SCADAHMI_API.Contracts.DTOs;

public record LightsStateDto(
    Guid Guid,
    string Name,
    bool IsDimmed,
    ushort MinimumBrightness,
    ushort MaximumBrightness,
    byte Red,
    byte Green,
    byte Blue
) : DeviceStateDto(Guid, Name)
{
    public LightsStateDto(Lights device) :
        this(device.Guid, device.Name, device.IsDimmed,
        device.MinimumBrightness, device.MaximumBrightness,
        device.Red, device.Green, device.Blue) {}
}