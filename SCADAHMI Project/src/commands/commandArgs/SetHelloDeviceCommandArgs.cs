using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command arguments for SetHelloDevice
/// </summary>
/// <param name="Message"></param>
public sealed record SetHelloDeviceCommandArgs(
    string Message = "Hello World!"
) : CommandArgs;
