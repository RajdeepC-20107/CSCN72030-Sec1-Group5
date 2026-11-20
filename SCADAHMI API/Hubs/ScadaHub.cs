using Microsoft.AspNetCore.SignalR;
using SCADAHMI_API.Contracts.DTOs;
using SCADAHMI_API.Contracts.Hubs;
using SCADAHMI_API.Services;
using SCADAHMI_Project;

namespace SCADAHMI_API.Hubs;

public class ScadaHub(DeviceRegistry registry) : Hub<IDeviceClient>
{
    public override async Task OnConnectedAsync()
    {
        DeviceSnapshotDto snapshot =
            DeviceSnapshotFactory.Create(registry, registry.Version);

        await Clients.Caller.DevicesChanged(snapshot);
        await base.OnConnectedAsync();
    }
}