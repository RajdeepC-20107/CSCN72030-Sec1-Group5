namespace SCADAHMI_Project;

/// <summary>
/// Record describing no arguments.
/// </summary>
public record RenameDeviceCommandArgs(
    Dictionary<Device, string> DeviceNames
) : CommandArgs;
