using Microsoft.AspNetCore.SignalR;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Contracts.Hubs;
using SCADAHMI_API.Hubs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Bridges core command execution to SignalR clients.
/// </summary>
public sealed class DeviceStateNotifier(
    IHubContext<ScadaHub, IDeviceClient> hubs,
    ILogger<DeviceStateNotifier> logger)
{
    public async Task PublishAsync(DeviceRegistry registry, int version)
    {
        try
        {
            DeviceSnapshotDto snapshot = DeviceSnapshotFactory.Create(registry, version);
            await hubs.Clients.All.DevicesChanged(snapshot);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not broadcast the device state change.");
        }
    }
}
