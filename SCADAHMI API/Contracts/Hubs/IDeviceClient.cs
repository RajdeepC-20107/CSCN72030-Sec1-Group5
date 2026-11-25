using SCADAHMI_API.Contracts.DTOs;

namespace SCADAHMI_API.Contracts.Hubs;

/// <summary>
/// The methods the server may call on a connected client. Used as the typed
/// hub contract for ScadaHub so calls are checked at compile time.
/// </summary>
public interface IDeviceClient
{
    /// <summary>
    /// Receives a snapshot after connecting or after a registry version change.
    /// </summary>
    /// <param name="snapshot"></param>
    Task DevicesChanged(DeviceSnapshotDto snapshot);
}
