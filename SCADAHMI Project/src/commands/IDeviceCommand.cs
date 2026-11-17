using System;

namespace SCADAHMI_Project;

public interface IDeviceCommand<TCommandArgs> : ICommand
    where TCommandArgs : CommandArgs
{
    HashSet<string> DeviceGUIDs { get; }
    void Execute(TCommandArgs args, HashSet<Device> devices);
}