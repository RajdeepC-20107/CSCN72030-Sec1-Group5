using System;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

public static class DeviceStateDtoFactory
{
    public static DeviceStateDto Create(Device device) => device switch
    {
        HelloDevice hello => new DeviceStateDto(hello.Guid, nameof(HelloDevice), hello.Message),
        _ => new DeviceStateDto(device.Guid, device.GetType().Name, "")
    };
}