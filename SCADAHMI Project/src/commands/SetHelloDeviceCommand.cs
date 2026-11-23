using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set all HelloDevices' message values.
/// </summary>
public sealed class SetHelloDeviceCommand
    : IDeviceCommand<SetHelloDeviceCommandArgs>
{
    bool IDeviceCommand<SetHelloDeviceCommandArgs>.Execute(
        DeviceRegistry deviceRegistry, 
        SetHelloDeviceCommandArgs args
    ) {
        var devices = deviceRegistry.GetDevicesOfType<HelloDevice>()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(var (device, message) in args.DeviceMessages)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)) {
                subscribed.Message = message;
                changed = true;
            }
        }

        return changed;
    }
}
