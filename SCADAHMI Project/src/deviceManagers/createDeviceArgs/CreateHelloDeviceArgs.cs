using System;

namespace SCADAHMI_Project;

/// <summary>
/// Arguments for creating a HelloDevice object.
/// </summary>
/// <param name="Message"></param>
public record CreateHelloDeviceArgs(
    string? Message = null
) : CreateDeviceArgs;
