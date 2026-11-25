using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Factory for device snapshot DTOs.
/// Maps domain devices to transport-neutral DTOs so domain types never
/// leak into the wire contract.
/// </summary>
public static class DeviceSnapshotFactory
{
    /// <summary>
    /// Creates a versioned snapshot of every subscribed device.
    /// </summary>
    /// <param name="registry"></param>
    /// <param name="version"></param>
    /// <returns>A snapshot carrying the given version and all current device states.</returns>
    public static DeviceSnapshotDto Create(DeviceRegistry registry, int version) =>
        new(
            version,
            registry.GetDevices()
                .Select(DeviceStateDtoFactory.Create)
                .ToList()
        );
}
