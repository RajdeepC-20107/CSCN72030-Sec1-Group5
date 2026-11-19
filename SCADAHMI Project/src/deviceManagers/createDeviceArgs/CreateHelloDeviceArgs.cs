using System;

namespace SCADAHMI_Project;

public record CreateHelloDeviceArgs(
    string? Message = null
) : CreateDeviceArgs;
