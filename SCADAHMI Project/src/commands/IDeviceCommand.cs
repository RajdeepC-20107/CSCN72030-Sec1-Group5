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
    /// <returns>Whether a version change should be called or not.</returns>
    bool Execute(DeviceRegistry deviceRegistry, TCommandArgs args);
}