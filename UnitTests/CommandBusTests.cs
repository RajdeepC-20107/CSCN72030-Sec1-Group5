namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class CommandBusTests
{
    [TestMethod]
    public void SetHelloDeviceMessages()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();

        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Subscribe(helloDevice2);

        string newMessage = "A new message!";

        // Act
        CommandBus.Publish(
            new SetHelloDeviceCommand(),
            new SetHelloDeviceCommandArgs(
                [(helloDevice1, newMessage),
                (helloDevice2, newMessage)]
            ),
            deviceRegistry
        );

        // Assert
        Assert.AreEqual(newMessage, helloDevice1.Message);
        Assert.AreEqual(newMessage, helloDevice2.Message);
    }

    // This test would be good to use Moq on
    // But for the sake of getting it done, I have not
    [TestMethod]
    public void RaisesCommandExecutedEventAndBumpsVersion()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice = new();
        deviceRegistry.Subscribe(helloDevice);
        CommandExecutedEventArgs? captured = null;
        int versionBefore = deviceRegistry.Version;

        void Handler(object? sender, CommandExecutedEventArgs e)
        {
            if (ReferenceEquals(e.Registry, deviceRegistry))
            {
                captured = e;
            }
        }

        CommandBus.CommandExecuted += Handler;

        // Act
        try
        {
            CommandBus.Publish(
                new SetHelloDeviceCommand(),
                new SetHelloDeviceCommandArgs(
                    [(helloDevice, "Event!")]
                ),
                deviceRegistry);
        }
        finally
        {
            CommandBus.CommandExecuted -= Handler;
        }

        // Assert
        Assert.IsNotNull(captured);
        Assert.AreEqual(versionBefore + 1, captured.Version);
        Assert.AreEqual(deviceRegistry.Version, captured.Version);
    }
}
