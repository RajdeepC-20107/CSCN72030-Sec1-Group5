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

    [TestMethod]
    public void VersionChangesOnSubscribeAndUnsubscribe()
    {
        // Arrange
        DeviceRegistry deviceRegistry = new();
        HelloDevice helloDevice = new();
        int versionBefore = deviceRegistry.Version;

        // Act & Assert
        Assert.IsTrue(deviceRegistry.Subscribe(helloDevice));
        Assert.AreEqual(versionBefore + 1, deviceRegistry.Version);

        // A duplicate subscribe must not bump the revision.
        Assert.IsFalse(deviceRegistry.Subscribe(helloDevice));
        Assert.AreEqual(versionBefore + 1, deviceRegistry.Version);

        Assert.IsTrue(deviceRegistry.Unsubscribe(helloDevice));
        Assert.AreEqual(versionBefore + 2, deviceRegistry.Version);
    }
}
