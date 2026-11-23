namespace SCADAHMI_Project;

/// <summary>
/// Record describing no arguments.
/// </summary>
public record SetDevicePowerCommandArgs(
    Dictionary<Device, bool> DevicePowerStates
) : CommandArgs;
