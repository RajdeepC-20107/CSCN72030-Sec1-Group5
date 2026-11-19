namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class HelloDeviceManagerTests
{
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
            ? HelloDeviceManager.RemoveDevice(helloDevice1, deviceRegistry)
            : null;
        bool? success2 = (helloDevice2 is not null) 
            ? HelloDeviceManager.RemoveDevice(helloDevice2, deviceRegistry)
            : null;
        bool? fail = (helloDevice1 is not null) 
            ? HelloDeviceManager.RemoveDevice(helloDevice1, deviceRegistry)
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

    [TestMethod]
    public void ChangeHelloDeviceMessages()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice? helloDevice1 = HelloDeviceManager.CreateDevice(new(), deviceRegistry)!;
        HelloDevice? helloDevice2 = HelloDeviceManager.CreateDevice(new(), deviceRegistry)!;
        HelloDevice? helloDevice3 = HelloDeviceManager.CreateDevice(new(), deviceRegistry)!;
        string message1 = "New Message!";
        string message2 = helloDevice2.Message;
        string message3 = "New Message...";

        // Act
        HelloDeviceManager.SetHelloDeviceMessages(
            [
                (helloDevice1, message1),
                (helloDevice3, message3)
            ],
            deviceRegistry
        );
        
        // Assert
        Assert.AreEqual(message1, helloDevice1.Message);
        Assert.AreEqual(message2, helloDevice2.Message);
        Assert.AreEqual(message3, helloDevice3.Message);
    }
}