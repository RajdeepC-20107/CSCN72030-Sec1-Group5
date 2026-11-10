namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class CommandHandlerTests
{
    [TestMethod]
    public void SetHelloDeviceMessages()
    {
        // Arrange
        DeviceListener deviceListener = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();

        deviceListener.Subscribe(helloDevice1);
        deviceListener.Subscribe(helloDevice2);

        string newMessage = "A new message!";

        // Act
        CommandHandler.Publish(
            deviceListener, 
            new SetHelloDeviceCommand(),
            new SetHelloDeviceCommandArgs(newMessage)
        );

        // Assert
        Assert.AreEqual(newMessage, helloDevice1.Message);
        Assert.AreEqual(newMessage, helloDevice2.Message);
    }
}