using Microsoft.AspNetCore.SignalR;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Contracts.Hubs;
using SCADAHMI_API.Hubs;
using SCADAHMI_Project;

namespace SCADAHMI_API.Services;

/// <summary>
/// Bridges registry changes to SignalR clients.
/// </summary>
/// <param name="hubs">Typed hub context used to reach all connected clients.</param>
/// <param name="logger">Used to record failed broadcasts.</param>
public sealed class DeviceStateNotifier(
    IHubContext<ScadaHub, IDeviceClient> hubs,
    ILogger<DeviceStateNotifier> logger)
{
    /// <summary>
    /// Builds a snapshot at the given version and pushes it to every connected
    /// client. Broadcast errors are logged rather than thrown.
    /// </summary>
    /// <param name="registry"></param>
    /// <param name="version"></param>
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
