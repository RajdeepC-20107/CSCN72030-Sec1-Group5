namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class DeviceRegistryTests
{
    [TestMethod]
    public void SubscribeHelloDevices()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();
        int expectedListCount = 2;

        // Act
        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Subscribe(helloDevice2);
        deviceRegistry.Subscribe(helloDevice1);

        var list = deviceRegistry.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.HasCount(expectedListCount, list);
        Assert.Contains(helloDevice1, list);
        Assert.Contains(helloDevice2, list);
    }

    [TestMethod]
    public void UnsubscribeHelloDevices()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();

        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Subscribe(helloDevice2);

        // Act
        deviceRegistry.Unsubscribe(helloDevice1);
        deviceRegistry.Unsubscribe(helloDevice2);
        deviceRegistry.Unsubscribe(helloDevice1);

        var list = deviceRegistry.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.IsEmpty(list);
    }
}