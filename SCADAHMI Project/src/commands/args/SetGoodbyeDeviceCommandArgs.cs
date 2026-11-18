using System;

namespace SCADAHMI_Project;

public sealed record SetGoodbyeDeviceCommandArgs(
    string Message
) : CommandArgs;
