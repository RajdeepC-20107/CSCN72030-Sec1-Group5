using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set all HelloDevices' message values.
/// </summary>
public sealed class RenameDeviceCommand
    : IDeviceCommand<RenameDeviceCommandArgs>
{
    public bool Execute(DeviceRegistry deviceRegistry, RenameDeviceCommandArgs args)
    {
        var devices = deviceRegistry.GetDevices()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(var (device, newName) in args.DeviceNames)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)
                && subscribed.Name != newName) {
                subscribed.Name = newName;
                changed = true;
            }
        }

        return changed;
    }
}
