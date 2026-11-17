using System;

namespace SCADAHMI_Project;

public sealed record SetHelloDeviceCommandArgs(
    string Message
) : CommandArgs;
