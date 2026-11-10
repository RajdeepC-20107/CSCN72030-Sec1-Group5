using System;

namespace SCADAHMI_Project;

public interface DeviceCommand<TCommandArgs>
    where TCommandArgs : CommandArgs
{
    void execute(DeviceListener deviceListener, TCommandArgs args);
}