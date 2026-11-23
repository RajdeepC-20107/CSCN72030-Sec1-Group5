using System;

namespace SCADAHMI_Project;

/// <summary>
/// The first device class to make sure this whole thing works.
/// </summary>
public sealed class HelloDevice : Device
{
    public override string Name { get; set; } = nameof(HelloDevice);

    /// <summary>
    /// The message of the HelloDevice.
    /// </summary>
    public string Message { get; set; } = "Hello World!";

    /// <summary>
    /// Supposedly, Devices will know how to affect and set their own values.
    /// This would be called via a command.Execute() method.
    /// Ex. If a speaker that is directed to B2 must now direct to F3,
    ///     a method like this with fed arguments will know how to 
    ///     change its variables and apply calculations as required.
    /// </summary>
    public void PrintMessage() => Console.WriteLine(Message);
}
