using System;

namespace SCADAHMI_Project;

public record CreateLightsArgs(
    ushort? MinimumBrightness,
    ushort? MaximumBrightness,
    byte? Red,
    byte? Green,
    byte? Blue
) : CreateDeviceArgs;
