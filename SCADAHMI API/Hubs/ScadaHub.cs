using Microsoft.AspNetCore.SignalR;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Contracts.Hubs;
using SCADAHMI_API.Services;
using SCADAHMI_Project;

namespace SCADAHMI_API.Hubs;

/// <summary>
/// The SignalR endpoint clients connect to. It is typed with IDeviceClient,
/// which declares the one method the server may call on a client. On
/// connection, the new client receives the current snapshot so it does not
/// have to wait for the next version change; all later broadcasts are done
/// by DeviceStateNotifier instead.
/// </summary>
public class ScadaHub(DeviceRegistry registry) : Hub<IDeviceClient>
{
    /// <summary>
    /// Sends the current snapshot to the newly connected client only.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        DeviceSnapshotDto snapshot =
            DeviceSnapshotFactory.Create(registry, registry.Version);

        await Clients.Caller.DevicesChanged(snapshot);
        await base.OnConnectedAsync();
    }
}