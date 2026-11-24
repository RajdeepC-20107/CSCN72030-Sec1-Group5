namespace SCADAHMI_Project;

/// <summary>
/// Command arguments for renaming a device.
/// </summary>
public record RenameDeviceCommandArgs(
    Dictionary<Device, string> DeviceNames
) : CommandArgs;
