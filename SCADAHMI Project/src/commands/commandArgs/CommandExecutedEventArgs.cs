namespace SCADAHMI_Project;

/// <summary>
/// Raised after a command has been executed by the bus.
/// </summary>
/// <param name="Command">The command that ran.</param>
/// <param name="Args">The arguments passed to the command.</param>
/// <param name="Registry">The registry the command operated on.</param>
/// <param name="Version">The registry revision after execution.</param>
public sealed record CommandExecutedEventArgs(
    ICommand Command,
    CommandArgs Args,
    DeviceRegistry Registry,
    int Version
);