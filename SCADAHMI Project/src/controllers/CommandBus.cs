using System;

namespace SCADAHMI_Project;

/// <summary>
/// Class that controls the publishing and 
/// processing of commands.
/// </summary>
public static class CommandBus
{
    /// <summary>
    /// The message/command bus. Stores commands and
    /// their associated arguments to be processed.
    /// </summary>
    private static readonly Queue<(ICommand, CommandArgs)> messageBus = [];

    /// <summary>
    /// Allows the publishing of commands that will eventually execute.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TCommandArgs"></typeparam>
    /// <param name="deviceRegistry"></param>
    /// <param name="command"></param>
    /// <param name="commandArgs"></param>
    public static void Publish<TCommand, TCommandArgs>(
        TCommand command,
        TCommandArgs commandArgs,
        DeviceRegistry? registry = null
    )
        where TCommandArgs : CommandArgs
        where TCommand : IDeviceCommand<TCommandArgs>
    {
        registry ??= DeviceRegistry.Instance;
        messageBus.Enqueue((command, commandArgs));
        (TCommand, TCommandArgs) poppedCommand = ((TCommand, TCommandArgs))messageBus.Dequeue();
        poppedCommand.Item1.Execute(registry, poppedCommand.Item2);
    }
}
