using System;

namespace SCADAHMI_Project;

public sealed class PrintHelloDeviceMessageCommand : IDeviceCommand<NoArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(NoArgs args, HashSet<Device> devices)
    {
        foreach (HelloDevice device in devices) 
        {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                Console.WriteLine(device.Message);
            }
        }
    }
}
