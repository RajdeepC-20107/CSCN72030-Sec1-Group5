using SCADAHMI_Project;

namespace SCADAHMI_API.Contracts.DTOs;

/// <summary>
/// Data transfer object for Lights objects.
/// </summary>
/// <param name="Guid"></param>
/// <param name="Name"></param>
/// <param name="IsDimmed"></param>
/// <param name="MinimumBrightness"></param>
/// <param name="MaximumBrightness"></param>
/// <param name="Red"></param>
/// <param name="Green"></param>
/// <param name="Blue"></param>
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