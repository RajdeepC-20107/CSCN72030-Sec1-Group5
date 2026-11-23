using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set all HelloDevices' message values.
/// </summary>
public sealed class PrintHelloDeviceCommand
    : IDeviceCommand<NoCommandArgs>
{
    bool IDeviceCommand<NoCommandArgs>.Execute(
        DeviceRegistry deviceRegistry, 
        NoCommandArgs args
    ) {
        IReadOnlyList<HelloDevice> devices =
            deviceRegistry.GetDevicesOfType<HelloDevice>();

        foreach(HelloDevice device in devices)
        {
            Console.WriteLine(device.Message);
        }

        return false;
    }
}
