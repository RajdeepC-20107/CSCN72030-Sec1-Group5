using SCADAHMI_Project;

namespace SCADAHMI_Project;

/// <summary>
/// Manual smoke test for the domain model. Kept as a plain class so the
/// core project can be consumed as a library by the API.
/// </summary>
public static class Demo
{
    public static void Run()
    {
        DeviceRegistry deviceRegistry = new();

        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();

        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Subscribe(helloDevice2);
        deviceRegistry.Subscribe(helloDevice1);

        Console.WriteLine(
            string.Join(", ", deviceRegistry.GetSubscribedDevices<HelloDevice>())
        );

        deviceRegistry.Unsubscribe(helloDevice1);
        deviceRegistry.Unsubscribe(helloDevice2);
        deviceRegistry.Unsubscribe(helloDevice1);

        Console.WriteLine(
            string.Join(", ", deviceRegistry.GetSubscribedDevices<HelloDevice>())
        );

        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Subscribe(helloDevice2);

        CommandBus.Publish(
            new SetHelloDeviceCommand(),
            new SetHelloDeviceCommandArgs("No way!"),
            deviceRegistry
        );
    }
}
