using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set all HelloDevices' message values.
/// </summary>
public sealed class SetHelloDeviceCommand
    : IDeviceCommand<SetHelloDeviceCommandArgs>
{
    void IDeviceCommand<SetHelloDeviceCommandArgs>.Execute(
        DeviceRegistry deviceRegistry, 
        SetHelloDeviceCommandArgs args
    ) {
        IReadOnlyList<HelloDevice> devices =
            deviceRegistry.GetDevicesOfType<HelloDevice>();

        foreach(HelloDevice device in devices)
        {
            device.Message = args.Message;
        }
    }
}
