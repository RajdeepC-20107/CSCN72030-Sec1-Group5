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

/*
[TestMethod]
    public void SubscribeHelloDevices()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        int expectedListCount = 2;

        // Act
        HelloDevice? helloDevice1 = HelloDeviceManager.CreateDevice(new(), deviceRegistry);
        HelloDevice? helloDevice2 = HelloDeviceManager.CreateDevice(new(), deviceRegistry);
        var list = deviceRegistry.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.IsNotNull(helloDevice1);
        Assert.IsNotNull(helloDevice2);
        Assert.HasCount(expectedListCount, list);
        Assert.Contains(helloDevice1, list);
        Assert.Contains(helloDevice2, list);
    }

    [TestMethod]
    public void UnsubscribeHelloDevices()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice? helloDevice1 = HelloDeviceManager.CreateDevice(new(), deviceRegistry);
        HelloDevice? helloDevice2 = HelloDeviceManager.CreateDevice(new(), deviceRegistry);

        // Act
        bool? success1 = (helloDevice1 is not null) 
            ? HelloDeviceManager.RemoveDevice(helloDevice1)
            : null;
        bool? success2 = (helloDevice2 is not null) 
            ? HelloDeviceManager.RemoveDevice(helloDevice2)
            : null;
        bool? fail = (helloDevice1 is not null) 
            ? HelloDeviceManager.RemoveDevice(helloDevice1)
            : null;
        var list = deviceRegistry.GetSubscribedDevices<HelloDevice>();

        // Assert
        Assert.IsNotNull(success1);
        Assert.IsTrue(success1);
        Assert.IsNotNull(success2);
        Assert.IsTrue(success2);
        Assert.IsNotNull(fail);
        Assert.IsFalse(fail);
        Assert.IsEmpty(list);
    }
*/