using System;

namespace SCADAHMI_Project;

public static class CommandHandler
{
    private static readonly List<ICommand> messageBus = [];

    public static void Publish<TCommand, TCommandArgs>(
        DeviceListener deviceListener, 
        TCommand command,
        TCommandArgs commandArgs
    )
        where TCommandArgs : CommandArgs
        where TCommand : IDeviceCommand<TCommandArgs>
    {
        messageBus.Add(command);
        command.Execute(deviceListener, commandArgs);
        messageBus.Remove(command);
    }
}
