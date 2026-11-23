namespace SCADAHMI_API.Contracts.DTOs;

public record HelloDeviceStateDto(
    Guid Guid,
    string Message
) : DeviceStateDto(Guid);