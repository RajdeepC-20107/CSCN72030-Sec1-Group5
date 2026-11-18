using System;

namespace SCADAHMI_Project;

public sealed class SetHelloDeviceCommand
    : IDeviceCommand<SetHelloDeviceCommandArgs>
{
    public HashSet<string>? DeviceGUIDs { get; set; } = null;

    public HashSet<DeviceType>? DeviceTypes { get; set; } = null;

    public void Execute(SetHelloDeviceCommandArgs args, HashSet<Device> devices)
    {
        foreach (HelloDevice device in devices) {
            if ((DeviceGUIDs == null || DeviceGUIDs.Contains(device.GUID.ToString()))
             && (DeviceTypes == null || DeviceTypes.Contains(device.Type)))
            {
                device.Message = args.Message;
            }
        }
    }
}
