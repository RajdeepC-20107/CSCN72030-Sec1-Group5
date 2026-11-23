using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Maps domain devices to transport-neutral DTOs so domain types never
/// leak into the wire contract.
/// </summary>
public static class DeviceSnapshotFactory
{
    public static DeviceSnapshotDto Create(DeviceRegistry registry, int version) =>
        new(
            version,
            registry.GetDevices()
                .Select(DeviceStateDtoFactory.Create)
                .ToList()
        );
}
