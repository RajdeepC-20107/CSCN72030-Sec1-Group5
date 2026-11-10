using System;

namespace SCADAHMI_Project;

public interface IDeviceCommand<TCommandArgs> : ICommand
    where TCommandArgs : CommandArgs
{
    void Execute(DeviceListener deviceListener, TCommandArgs args);
}