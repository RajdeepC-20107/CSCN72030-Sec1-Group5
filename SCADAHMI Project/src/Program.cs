using SCADAHMI_Project;

namespace SCADAHMI_Project;

class Program
{

    static CmdMessageBus bus = new CmdMessageBus();
    static void Main()
    {
        HelloDeviceOutput();
        GoodbyeDeviceOutput();
        CalcOutput();
    }

    static void HelloDeviceOutput()
    {
        HelloDevice device1 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Hello Device 1"
        };

        HelloDevice device2 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Hello Device 2"
        };

        HelloDevice device3 = new HelloDevice
        {
            GUID = Guid.NewGuid(),
            Name = "Hello Device 3"
        };

        bus.AddDevice(device1);
        bus.AddDevice(device2);
        bus.AddDevice(device3);

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
            new SetHelloDeviceCommandArgs("Hello, Amigo!");

        bus.Enqueue(command, args);

        Thread.Sleep(500);

        PrintHelloDeviceMessageCommand printCommand = new PrintHelloDeviceMessageCommand {
            DeviceGUIDs = new HashSet<string>
            {
                device1.GUID.ToString(),
                device2.GUID.ToString(),
                device3.GUID.ToString()
            }
        };
        
        bus.Enqueue(printCommand, new NoArgs());
    }

    static void GoodbyeDeviceOutput()
    {
        GoodbyeDevice goodbyeDevice1 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Goodbye Device 1"
        };

        GoodbyeDevice goodbyeDevice2 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Goodbye Device 2"
        };

        GoodbyeDevice goodbyeDevice3 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Goodbye Device 3"
        };

        bus.AddDevice(goodbyeDevice1);
        bus.AddDevice(goodbyeDevice2);
        bus.AddDevice(goodbyeDevice3);

        SetGoodbyeDeviceCommand setGoodbyeDeviceCommand1 = new()
        {
            DeviceGUIDs = new HashSet<string>
            {
                goodbyeDevice1.GUID.ToString(),
                goodbyeDevice2.GUID.ToString()
            }
        };
        SetGoodbyeDeviceCommand setGoodbyeDeviceCommand2 = new()
        {
            DeviceGUIDs = new HashSet<string>
            {
                goodbyeDevice2.GUID.ToString(),
                goodbyeDevice3.GUID.ToString()
            }
        };

        SetGoodbyeDeviceCommandArgs args1 = new("Not so goodbye");
        SetGoodbyeDeviceCommandArgs args2 = new("So very goodbye");

        bus.Enqueue(setGoodbyeDeviceCommand1, args1);
        bus.Enqueue(setGoodbyeDeviceCommand2, args2);

        Thread.Sleep(500);

        PrintGoodbyeDeviceMessageCommand printCommand = new PrintGoodbyeDeviceMessageCommand {
            DeviceGUIDs = new HashSet<string>
            {
                goodbyeDevice1.GUID.ToString(),
                goodbyeDevice2.GUID.ToString(),
                goodbyeDevice3.GUID.ToString()
            }
        };
        
        bus.Enqueue(printCommand, new NoArgs());
    }

    static void CalcOutput()
    {
        CalculatorDevice calculator1 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Calc 1"
        };

        CalculatorDevice calculator2 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Calc 2"
        };

        CalculatorDevice calculator3 = new()
        {
            GUID = Guid.NewGuid(),
            Name = "Calc 3"
        };

        bus.AddDevice(calculator1);
        bus.AddDevice(calculator2);
        bus.AddDevice(calculator3);

        SetCalculatorValuesCommand command1 = new()
        {
            DeviceGUIDs = new HashSet<string>
            {
                calculator2.GUID.ToString()
            }
        };

        SetCalculatorValueCommandArgs args1 = new(47, 53);
        bus.Enqueue(command1, args1);

        SetCalculatorValuesCommand command2 = new()
        {
            DeviceGUIDs = new HashSet<string>
            {
                calculator3.GUID.ToString()
            }
        };
        SetCalculatorValueCommandArgs args2 = new(407, 553);

        bus.Enqueue(command2, args2);

        bus.RemoveDevice(calculator1.GUID);

        PrintCalculatorSumCommand printCommand = new()
        {
            DeviceGUIDs = new HashSet<string>
            {
                calculator1.GUID.ToString(),
                calculator2.GUID.ToString(),
                calculator3.GUID.ToString()
            }
        };

        Thread.Sleep(500);

        bus.Enqueue(printCommand, new NoArgs());
    }
}