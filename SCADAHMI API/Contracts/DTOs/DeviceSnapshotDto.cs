namespace SCADAHMI_API.Contracts.DTOs;

/// <summary>
/// A versioned snapshot of all devices. Used by both the poll endpoint
/// and the SignalR "devicesChanged" event.
/// </summary>
public sealed record DeviceSnapshotDto(
    int Version,
    IReadOnlyList<DeviceStateDto> Devices
);
