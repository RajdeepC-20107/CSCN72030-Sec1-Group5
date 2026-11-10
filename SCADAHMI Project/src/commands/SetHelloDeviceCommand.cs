using System;

namespace SCADAHMI_Project;

public sealed class SetHelloDeviceCommand
    : IDeviceCommand<SetHelloDeviceCommandArgs>
{
    void IDeviceCommand<SetHelloDeviceCommandArgs>.Execute(
        DeviceListener deviceListener, 
        SetHelloDeviceCommandArgs args
    ) {
        IReadOnlyList<HelloDevice> devices =
            deviceListener.GetSubscribedDevices<HelloDevice>();

        foreach(HelloDevice device in devices)
        {
            device.Message = args.Message;
        }
    }
}
