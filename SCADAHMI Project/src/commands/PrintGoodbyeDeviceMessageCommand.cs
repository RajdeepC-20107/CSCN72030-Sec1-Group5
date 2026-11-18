using System;

namespace SCADAHMI_Project;

public sealed class PrintGoodbyeDeviceMessageCommand : IDeviceCommand<NoArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(NoArgs args, HashSet<Device> devices)
    {
        foreach (GoodbyeDevice device in devices) 
        {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                Console.WriteLine(device.Message);
            }
        }
    }
}
