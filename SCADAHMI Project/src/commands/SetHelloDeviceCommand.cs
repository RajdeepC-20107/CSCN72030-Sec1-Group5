using System;

namespace SCADAHMI_Project;

public sealed class SetHelloDeviceCommand
    : IDeviceCommand<SetHelloDeviceCommandArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(SetHelloDeviceCommandArgs args, HashSet<Device> devices)
    {
        foreach (HelloDevice device in devices) {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                device.Message = args.Message;
            }
        }
    }
}
