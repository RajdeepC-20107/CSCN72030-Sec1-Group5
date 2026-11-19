namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class CommandHandlerTests
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
            deviceRegistry, 
            new SetHelloDeviceCommand(),
            new SetHelloDeviceCommandArgs(newMessage)
        );

        // Assert
        Assert.AreEqual(newMessage, helloDevice1.Message);
        Assert.AreEqual(newMessage, helloDevice2.Message);
    }
}