using System;

namespace SCADAHMI_Project;

public sealed class SetCalculatorValuesCommand : IDeviceCommand<SetCalculatorValueCommandArgs>
{
    public required HashSet<string> DeviceGUIDs { get; set; }

    public void Execute(SetCalculatorValueCommandArgs args, HashSet<Device> devices)
    {
        foreach (CalculatorDevice device in devices) 
        {
            if (DeviceGUIDs.Contains(device.GUID.ToString()))
            {
                device.SetValues(args.Value1, args.Value2);
            }
        }
    }
}
