using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command arguments for setting the messages of HelloDevices.
/// </summary>
/// <param name="DeviceMessages"></param>
public sealed record SetHelloDeviceCommandArgs(
    List<(HelloDevice, string)> DeviceMessages
) : CommandArgs;
