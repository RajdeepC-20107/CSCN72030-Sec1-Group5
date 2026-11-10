using System;

namespace SCADAHMI_Project;

public class HelloDevice : Device
{
    public override void onPublishEvent()
    {
        Console.WriteLine("Hello from this device!");
    }
}
