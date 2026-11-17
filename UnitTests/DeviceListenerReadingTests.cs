namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class DeviceListenerReadingTests
{
    [TestMethod]
    public void PollOncePublishesOneReadingPerDeviceInSubscriptionOrder()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        LightsManager lights = new("lights", new DataFileReader(path));
        ACManager ac = new("ac", new DataFileReader(path));

        DeviceListener deviceListener = new();
        deviceListener.Subscribe(lights);
        deviceListener.Subscribe(ac);
        deviceListener.Subscribe(lights); // duplicate is ignored

        List<DeviceReading> received = [];
        deviceListener.SubscribeReadings(received.Add);

        // Act
        deviceListener.PollOnce();

        // Assert
        Assert.HasCount(2, received);
        Assert.AreEqual("lights", received[0].DeviceId);
        Assert.AreEqual("ac", received[1].DeviceId);
    }

    [TestMethod]
    public void PublishReadingPassesTheReadingToEverySubscriber()
    {
        // Arrange
        DeviceListener deviceListener = new();
        List<DeviceReading> first = [];
        List<DeviceReading> second = [];
        deviceListener.SubscribeReadings(first.Add);
        deviceListener.SubscribeReadings(second.Add);

        DeviceReading reading = new("lights", DateTime.Now, 40, "%", DeviceStatus.Ok);

        // Act
        deviceListener.PublishReading(reading);

        // Assert
        Assert.AreEqual(reading, first.Single());
        Assert.AreEqual(reading, second.Single());
    }

    [TestMethod]
    public void DevicesWithoutReadAreNotPolled()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        DeviceListener deviceListener = new();
        deviceListener.Subscribe(new HelloDevice());
        deviceListener.Subscribe(new LightsManager("lights", new DataFileReader(path)));

        // Act
        IReadOnlyList<DeviceReading> readings = deviceListener.PollOnce();

        // Assert
        Assert.HasCount(1, readings);
        Assert.AreEqual("lights", readings[0].DeviceId);
    }

    [TestMethod]
    public void FaultedDeviceDoesNotStopTheOtherDevices()
    {
        // Arrange: the projector's file is missing, the others are fine.
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        LightsManager lights = new("lights", new DataFileReader(path));
        ProjectorManager projector = new(
            "projector", new DataFileReader(DataFileTestHelper.NewPath())
        );
        ACManager ac = new("ac", new DataFileReader(path));

        DeviceListener deviceListener = new();
        deviceListener.Subscribe(lights);
        deviceListener.Subscribe(projector);
        deviceListener.Subscribe(ac);

        // Act
        IReadOnlyList<DeviceReading> first = deviceListener.PollOnce();
        IReadOnlyList<DeviceReading> second = deviceListener.PollOnce();

        // Assert
        Assert.AreEqual(DeviceStatus.Ok, first[0].Status);
        Assert.AreEqual(DeviceStatus.Fault, first[1].Status);
        Assert.AreEqual(DeviceStatus.Ok, first[2].Status);

        // The healthy devices keep advancing through their files.
        Assert.AreEqual(2.5, second[0].Value);
        Assert.AreEqual(DeviceStatus.Fault, second[1].Status);
        Assert.AreEqual(2.5, second[2].Value);
    }

    [TestMethod]
    public void DeviceThatThrowsIsReportedAsFault()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        DeviceListener deviceListener = new();
        deviceListener.Subscribe(new ThrowingDevice());
        deviceListener.Subscribe(new LightsManager("lights", new DataFileReader(path)));

        // Act
        IReadOnlyList<DeviceReading> readings = deviceListener.PollOnce();

        // Assert
        Assert.AreEqual(DeviceStatus.Fault, readings[0].Status);
        Assert.AreEqual("throwing", readings[0].DeviceId);
        Assert.AreEqual(DeviceStatus.Ok, readings[1].Status);
    }

    [TestMethod]
    public void HandlerThatThrowsDoesNotStopOtherHandlers()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        DeviceListener deviceListener = new();
        deviceListener.Subscribe(new LightsManager("lights", new DataFileReader(path)));

        List<DeviceReading> received = [];
        deviceListener.SubscribeReadings(_ => throw new InvalidOperationException("bad subscriber"));
        deviceListener.SubscribeReadings(received.Add);

        // Act
        deviceListener.PollOnce();

        // Assert
        Assert.HasCount(1, received);
    }

    [TestMethod]
    public void UnsubscribedDeviceAndUnsubscribedHandlerAreNoLongerUsed()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        LightsManager lights = new("lights", new DataFileReader(path));
        ACManager ac = new("ac", new DataFileReader(path));

        DeviceListener deviceListener = new();
        deviceListener.Subscribe(lights);
        deviceListener.Subscribe(ac);

        List<DeviceReading> kept = [];
        List<DeviceReading> removed = [];
        Action<DeviceReading> removedHandler = removed.Add;
        deviceListener.SubscribeReadings(kept.Add);
        deviceListener.SubscribeReadings(removedHandler);

        // Act
        deviceListener.Unsubscribe(ac);
        deviceListener.UnsubscribeReadings(removedHandler);
        deviceListener.PollOnce();

        // Assert
        Assert.HasCount(1, kept);
        Assert.AreEqual("lights", kept[0].DeviceId);
        Assert.IsEmpty(removed);
    }

    [TestMethod]
    public void StartPublishesReadingsAtAFixedIntervalUntilStopped()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(3));
        using DeviceListener deviceListener = new();
        deviceListener.Subscribe(new LightsManager("lights", new DataFileReader(path)));

        List<DeviceReading> received = [];
        using SemaphoreSlim arrived = new(0);
        deviceListener.SubscribeReadings(reading =>
        {
            lock (received)
            {
                received.Add(reading);
            }

            arrived.Release();
        });

        // Act: wait for four readings, which wraps the three-line file once.
        deviceListener.Start(TimeSpan.FromMilliseconds(20));
        for (int i = 0; i < 4; i++)
        {
            Assert.IsTrue(arrived.Wait(TimeSpan.FromSeconds(5)), "No reading arrived in time.");
        }

        deviceListener.Stop();
        int countAfterStop;
        double[] firstFour;
        lock (received)
        {
            countAfterStop = received.Count;
            firstFour = received.Take(4).Select(r => r.Value).ToArray();
        }

        Thread.Sleep(100);

        // Assert
        CollectionAssert.AreEqual(new[] { 1.5, 2.5, 3.5, 1.5 }, firstFour);
        Assert.IsFalse(deviceListener.IsRunning);
        lock (received)
        {
            Assert.HasCount(countAfterStop, received);
        }
    }

    [TestMethod]
    public void StartRejectsANonPositiveInterval()
    {
        // Arrange
        using DeviceListener deviceListener = new();

        // Act + Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => deviceListener.Start(TimeSpan.Zero));
    }

    // A badly behaved device that breaks the "Read never throws" rule.
    private sealed class ThrowingDevice : Device, IDevice
    {
        public string DeviceId => "throwing";

        public DeviceReading Read() => throw new InvalidOperationException("sensor exploded");
    }
}
