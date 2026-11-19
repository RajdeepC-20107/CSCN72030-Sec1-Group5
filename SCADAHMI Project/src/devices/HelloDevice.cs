using System;

namespace SCADAHMI_Project;

public sealed class HelloDevice : Device
{
    public string Message { get; set; } = "Hello World!";

    /// <summary>
    /// Devices will know how to affect and set their own values.
    /// Ex. If a speaker that is directed to B2 must now direct to F3,
    ///     a method like this with fed argumenta will know how to 
    ///     change its variables and apply calculations as required.
    /// </summary>
    public void PrintMessage() => Console.WriteLine(Message);
}
