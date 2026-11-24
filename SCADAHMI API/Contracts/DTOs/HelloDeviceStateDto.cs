using SCADAHMI_Project;

namespace SCADAHMI_API.Contracts.DTOs;

/// <summary>
/// Data transfer object for HelloDevice objects.
/// </summary>
/// <param name="Guid"></param>
/// <param name="Name"></param>
/// <param name="Message"></param>
public record HelloDeviceStateDto(
    Guid Guid,
    string Name,
    string Message
) : DeviceStateDto(Guid, Name)
{
    public HelloDeviceStateDto(HelloDevice device)
        : this(device.Guid, device.Name, device.Message) {}
}