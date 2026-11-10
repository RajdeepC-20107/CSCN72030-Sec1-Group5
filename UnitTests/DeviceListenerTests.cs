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
}