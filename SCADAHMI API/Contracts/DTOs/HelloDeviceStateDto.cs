using SCADAHMI_Project;

namespace SCADAHMI_API.Contracts.DTOs;

public record HelloDeviceStateDto(
    Guid Guid,
    string Name,
    bool PoweredOn,
    string Message
) : DeviceStateDto(Guid, Name, PoweredOn)
{
    public HelloDeviceStateDto(HelloDevice device)
        : this(device.Guid, device.Name, device.PoweredOn, device.Message) {}
}