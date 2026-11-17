using SCADAHMI_Project;

class Program
{
    static void Main()
    {
        HelloDevice device1 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Device 1"
        };

        HelloDevice device2 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Device 2"
        };

        HelloDevice device3 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Device 3"
        };

        CmdMessageBus bus = new CmdMessageBus();

        bus.AddDevice(device1);
        bus.AddDevice(device2);
        bus.AddDevice(device3);

        bus.RemoveDevice(device2);

        SetHelloDeviceCommand command = new SetHelloDeviceCommand
        {
            DeviceGUIDs = new HashSet<string>
            {
                device1.GUID.ToString(),
                device2.GUID.ToString(),
                device3.GUID.ToString()
            }
        };

        SetHelloDeviceCommandArgs args =
            new SetHelloDeviceCommandArgs("Hello, World!");

        bus.Enqueue(command, args);

        Thread.Sleep(500);

        Console.WriteLine($"{device1.Name}: {device1.Message}");
        Console.WriteLine($"{device2.Name}: {device2.Message}");
        Console.WriteLine($"{device3.Name}: {device3.Message}");
    }
}