using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Factory for existing Device State DTOs.
/// Used to map different devices to device state DTOs.
/// </summary>
public static class DeviceStateDtoFactory
{
    /// <summary>
    /// Maps a domain device to its wire DTO.
    /// </summary>
    /// <param name="device"></param>
    /// <returns>The DTO matching the device type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the device has no wire type.</exception>
    public static DeviceStateDto Create(Device device) => device switch
    {
        HelloDevice hello => new HelloDeviceStateDto(hello),
        Lights lights => new LightsStateDto(lights),
        _ => 
            throw new NotSupportedException($"No wire type for {device.GetType().Name}.")
    };
}