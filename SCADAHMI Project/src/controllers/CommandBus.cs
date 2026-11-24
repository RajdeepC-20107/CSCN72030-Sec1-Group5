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
    /// Serializes bus access so concurrent clients cannot corrupt the queue
    /// or interleave command execution.
    /// </summary>
    private static readonly Lock gate = new();

    /// <summary>
    /// Raised after every published command finishes, even when it changed nothing.
    /// Carries the command, its arguments, the registry, and the revision after
    /// execution. SignalR broadcasts use DeviceRegistry.Changed instead, so this
    /// is for observers such as logging or tests.
    /// </summary>
    public static event EventHandler<CommandExecutedEventArgs>? CommandExecuted;

    /// <summary>
    /// Allows the publishing of commands that will eventually 
    /// execute asynchronously (hopefully).
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TCommandArgs"></typeparam>
    /// <param name="registry"></param>
    /// <param name="command"></param>
    /// <param name="commandArgs"></param>
    public static void Publish<TCommand, TCommandArgs>(
        TCommand command,
        TCommandArgs commandArgs,
        DeviceRegistry registry
    )
        where TCommandArgs : CommandArgs
        where TCommand : IDeviceCommand<TCommandArgs>
    {
        lock (gate)
        {
            messageBus.Enqueue((command, commandArgs));
            (TCommand, TCommandArgs) poppedCommand = ((TCommand, TCommandArgs))messageBus.Dequeue();
            bool versionChange = poppedCommand.Item1.Execute(registry, poppedCommand.Item2);

            if(versionChange)
            {
                registry.MarkChanged();
            }
            CommandExecuted?.Invoke(
                null,
                new CommandExecutedEventArgs(
                    poppedCommand.Item1, poppedCommand.Item2, registry, registry.Version
                )
            );
        }
    }
}
