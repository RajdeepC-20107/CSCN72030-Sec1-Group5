using System;

namespace SCADAHMI_Project;

public sealed class PrintCalculatorSumCommand : IDeviceCommand<NoArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(NoArgs args, HashSet<Device> devices)
    {
        foreach (CalculatorDevice device in devices) 
        {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                Console.WriteLine($"GUID {device.GUID} Sum: {device.GetSum()}");
            }
        }
    }
}
