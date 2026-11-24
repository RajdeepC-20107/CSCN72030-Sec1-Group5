using SCADAHMI_Project;

namespace SCADAHMI_API.Contracts.DTOs;

public record HelloDeviceStateDto(
    Guid Guid,
    string Name,
    string Message
) : DeviceStateDto(Guid, Name)
{
    public HelloDeviceStateDto(HelloDevice device)
        : this(device.Guid, device.Name, device.Message) {}
}