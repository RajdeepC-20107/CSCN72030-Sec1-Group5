using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set all HelloDevices' message values.
/// </summary>
public sealed class SetDevicePowerCommand
    : IDeviceCommand<SetDevicePowerCommandArgs>
{
    public bool Execute(DeviceRegistry deviceRegistry, SetDevicePowerCommandArgs args)
    {
        var devices = deviceRegistry.GetDevices()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(var (device, poweredOn) in args.DevicePowerStates)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)) {
                subscribed.PoweredOn = poweredOn;
                changed = true;
            }
        }

        return changed;
    }
}
