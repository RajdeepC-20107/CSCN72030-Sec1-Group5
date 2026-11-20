using System;

namespace SCADAHMI_Project;

/// <summary>
/// The interface used to define new commands.
/// </summary>
/// <typeparam name="TCommandArgs"></typeparam>
public interface IDeviceCommand<TCommandArgs> : ICommand
    where TCommandArgs : CommandArgs
{
    /// <summary>
    /// Method used for the command to perform an action.
    /// </summary>
    /// <param name="deviceRegistry"></param>
    /// <param name="args"></param>
    void Execute(DeviceRegistry deviceRegistry, TCommandArgs args);
}