using System;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

public static class DeviceStateDtoFactory
{
    public static DeviceStateDto Create(Device device) => device switch
    {
        HelloDevice hello => new HelloDeviceStateDto(hello),
        _ => 
            throw new NotSupportedException($"No wire type for {device.GetType().Name}.")
    };
}