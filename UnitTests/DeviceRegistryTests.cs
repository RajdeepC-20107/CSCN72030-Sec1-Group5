namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class deviceRegistryTests
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

    [TestMethod]
    public void SubscriptionOrderPreservation()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();
        HelloDevice helloDevice3 = new();
        HelloDevice helloDevice4 = new();
        int expectedListCount = 4;

        // Act
        deviceRegistry.Subscribe(helloDevice3);
        deviceRegistry.Subscribe(helloDevice2);
        deviceRegistry.Subscribe(helloDevice1);
        deviceRegistry.Unsubscribe(helloDevice2);
        deviceRegistry.Subscribe(helloDevice2);
        deviceRegistry.Subscribe(helloDevice4);

        var list = deviceRegistry.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.HasCount(expectedListCount, list);
        Assert.AreEqual(helloDevice3, list[0]);
        Assert.AreEqual(helloDevice1, list[1]);
        Assert.AreEqual(helloDevice2, list[2]);
        Assert.AreEqual(helloDevice4, list[3]);
    }
}