using System;

namespace SCADAHMI_Project;

public static class CommandHandler
{
    public static void Publish<TCommand, TCommandArgs>(
        DeviceListener deviceListener, 
        TCommand command,
        TCommandArgs commandArgs
    )
        where TCommandArgs: CommandArgs
        where TCommand:DeviceCommand<TCommandArgs>
    {
        command.execute(deviceListener, commandArgs);
    }
}
