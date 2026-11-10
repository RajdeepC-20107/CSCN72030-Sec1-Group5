namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class DeviceListenerTests
{
    [TestMethod]
    public void SubscribeHelloDevices()
    {
        // Arrange
        DeviceListener deviceListener = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();
        int expectedListCount = 2;

        // Act
        deviceListener.Subscribe(helloDevice1);
        deviceListener.Subscribe(helloDevice2);
        deviceListener.Subscribe(helloDevice1);

        var list = deviceListener.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.HasCount(expectedListCount, list);
        Assert.Contains(helloDevice1, list);
        Assert.Contains(helloDevice2, list);
    }

    [TestMethod]
    public void UnsubscribeHelloDevices()
    {
        // Arrange
        DeviceListener deviceListener = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();

        deviceListener.Subscribe(helloDevice1);
        deviceListener.Subscribe(helloDevice2);

        // Act
        deviceListener.Unsubscribe(helloDevice1);
        deviceListener.Unsubscribe(helloDevice2);
        deviceListener.Unsubscribe(helloDevice1);

        var list = deviceListener.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.IsEmpty(list);
    }

    [TestMethod]
    public void SubscriptionOrderPreservation()
    {
        // Arrange
        DeviceListener deviceListener = new();
        HelloDevice helloDevice1 = new();
        HelloDevice helloDevice2 = new();
        HelloDevice helloDevice3 = new();
        HelloDevice helloDevice4 = new();
        int expectedListCount = 4;

        // Act
        deviceListener.Subscribe(helloDevice3);
        deviceListener.Subscribe(helloDevice2);
        deviceListener.Subscribe(helloDevice1);
        deviceListener.Unsubscribe(helloDevice2);
        deviceListener.Subscribe(helloDevice2);
        deviceListener.Subscribe(helloDevice4);

        var list = deviceListener.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.HasCount(expectedListCount, list);
        Assert.AreEqual(helloDevice3, list[0]);
        Assert.AreEqual(helloDevice1, list[1]);
        Assert.AreEqual(helloDevice2, list[2]);
        Assert.AreEqual(helloDevice4, list[3]);
    }
}