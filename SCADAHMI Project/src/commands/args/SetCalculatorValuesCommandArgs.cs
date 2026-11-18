using System;

namespace SCADAHMI_Project;

public sealed record SetCalculatorValueCommandArgs(
    int? Value1,
    int? Value2
) : CommandArgs;
