using System;

namespace SCADAHMI_Project;

public sealed class SetGoodbyeDeviceCommand
    : IDeviceCommand<SetGoodbyeDeviceCommandArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(SetGoodbyeDeviceCommandArgs args, HashSet<Device> devices)
    {
        foreach (GoodbyeDevice device in devices) {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                device.Message = args.Message;
            }
        }
    }
}
