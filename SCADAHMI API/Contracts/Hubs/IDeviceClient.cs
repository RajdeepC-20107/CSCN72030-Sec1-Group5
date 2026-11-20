using System;
using SCADAHMI_API.Contracts.DTOs;

namespace SCADAHMI_API.Contracts.Hubs;

public interface IDeviceClient
{
    Task DevicesChanged(DeviceSnapshotDto snapshot);
}
