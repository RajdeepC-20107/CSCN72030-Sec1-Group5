using System;

namespace SCADAHMI_Project;

public class GoodbyeDevice : Device
{
    public string Message {get; set;} = "Goodbye";

    public void SendMessage() => Console.WriteLine(Message);
}
