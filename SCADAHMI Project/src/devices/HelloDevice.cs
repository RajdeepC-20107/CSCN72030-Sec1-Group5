using System;

namespace SCADAHMI_Project;

public sealed class HelloDevice : Device
{
    public string Message { get; set; } = "Empty message.";

    public void SendMessage() => Console.WriteLine(Message);
}
