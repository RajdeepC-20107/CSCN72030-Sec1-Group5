namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class DeviceManagerTests
{
    [TestMethod]
    public void PolledTenTimesReturnsOneReadingPerPollInFileOrder()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(10));
        LightsManager lights = new("lights", new DataFileReader(path));

        // Act
        List<DeviceReading> readings = [];
        for (int i = 0; i < 10; i++)
        {
            readings.Add(lights.Read());
        }

        // Assert
        Assert.HasCount(10, readings);
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i + 1.5, readings[i].Value);
            Assert.AreEqual(DeviceStatus.Ok, readings[i].Status);
            Assert.AreEqual("lights", readings[i].DeviceId);
        }
    }

    [TestMethod]
    public void PolledPastTheEndLoopsBackToLineOne()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(10));
        ACManager ac = new("ac", new DataFileReader(path));

        for (int i = 0; i < 10; i++)
        {
            ac.Read();
        }

        // Act
        DeviceReading eleventh = ac.Read();

        // Assert
        Assert.AreEqual(1.5, eleventh.Value);
        Assert.AreEqual(1.5, ac.Temperature);
    }

    [TestMethod]
    public void MissingFileReportsFaultWithoutThrowing()
    {
        // Arrange
        ProjectorManager projector = new(
            "projector", new DataFileReader(DataFileTestHelper.NewPath())
        );

        // Act
        DeviceReading reading = projector.Read();

        // Assert
        Assert.AreEqual(DeviceStatus.Fault, reading.Status);
        Assert.AreEqual(DeviceStatus.Fault, projector.Status);
        Assert.AreEqual("projector", reading.DeviceId);
        Assert.IsNotNull(reading.Detail);
    }

    [TestMethod]
    public void CorruptFileReportsFaultWithoutThrowing()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(
            "2026-11-16T19:00:00,40.0,%,Ok",
            "garbage"
        );
        StereoSpeakerManager speaker = new("speaker-center", new DataFileReader(path));

        // Act
        DeviceReading reading = speaker.Read();

        // Assert
        Assert.AreEqual(DeviceStatus.Fault, reading.Status);
        Assert.AreEqual(DeviceStatus.Fault, speaker.Status);
    }

    [TestMethod]
    public void FaultClearsOnceTheFileIsFixed()
    {
        // Arrange
        string path = DataFileTestHelper.NewPath();
        LightsManager lights = new("lights", new DataFileReader(path));
        Assert.AreEqual(DeviceStatus.Fault, lights.Read().Status);

        // Act
        File.WriteAllLines(path, DataFileTestHelper.ValidLines(2));
        DeviceReading reading = lights.Read();

        // Assert
        Assert.AreEqual(DeviceStatus.Ok, reading.Status);
        Assert.AreEqual(1.5, lights.DimLevel);
    }

    [TestMethod]
    public void StatusFromTheFileIsPassedThrough()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(
            "2026-11-16T19:00:00,80,%,Ok",
            "2026-11-16T19:00:01,0,%,NoPower"
        );
        ProjectorManager projector = new("projector", new DataFileReader(path));

        // Act
        projector.Read();
        DeviceReading second = projector.Read();

        // Assert: a non-Ok sample does not overwrite the last good value.
        Assert.AreEqual(DeviceStatus.NoPower, second.Status);
        Assert.AreEqual(80, projector.Brightness);
    }

    [TestMethod]
    public void SpeakerVolumeIsCappedAtMaxVolume()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile("2026-11-16T19:00:00,90,%,Ok");
        StereoSpeakerManager speaker = new("speaker-rear", new DataFileReader(path))
        {
            MaxVolume = 60
        };

        // Act
        speaker.Read();

        // Assert
        Assert.AreEqual(60, speaker.Volume);
    }

    [TestMethod]
    public void MotionSensorDecodesOccupiedSeats()
    {
        // Arrange: 5 is binary 101, so seats 1 and 3.
        string path = DataFileTestHelper.WriteFile("2026-11-16T19:00:00,5,seats,Ok");
        MotionSensorManager sensor = new("motion-sensor", new DataFileReader(path));

        // Act
        sensor.Read();

        // Assert
        CollectionAssert.AreEqual(new[] { 1, 3 }, sensor.OccupiedSeats.ToList());
    }

    [TestMethod]
    public void DevicesStillWorkWithTheExistingDeviceListener()
    {
        // Arrange
        DeviceListener deviceListener = new();
        LightsManager lights = new("lights", new DataFileReader(DataFileTestHelper.NewPath()));

        // Act
        deviceListener.Subscribe(lights);

        // Assert
        Assert.Contains(lights, deviceListener.GetSubscribedDevices<LightsManager>());
    }
}
