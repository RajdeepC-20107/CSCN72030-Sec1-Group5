using System;

namespace SCADAHMI_Project;

/// <summary>
/// Arguments for creating a Lights device object.
/// </summary>
/// <param name="MinimumBrightness"></param>
/// <param name="MaximumBrightness"></param>
/// <param name="Red"></param>
/// <param name="Green"></param>
/// <param name="Blue"></param>
public record CreateLightsArgs(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDeviceArgs;
