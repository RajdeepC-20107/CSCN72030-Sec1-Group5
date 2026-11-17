namespace UnitTests;

using SCADAHMI_Project;

[TestClass]
public sealed class DataFileReaderTests
{
    private const string DeviceId = "test-device";

    [TestMethod]
    public void TenLinesReturnTenReadingsInFileOrder()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(10));
        DataFileReader reader = new(path);

        // Act
        List<DeviceReading> readings = [];
        for (int i = 0; i < 10; i++)
        {
            readings.Add(reader.ReadNext(DeviceId));
        }

        // Assert
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i + 1.5, readings[i].Value);
            Assert.AreEqual(DeviceId, readings[i].DeviceId);
        }
    }

    [TestMethod]
    public void ReadingCarriesEveryFieldOfTheLine()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile("2026-11-16T19:00:05,21.5,C,NoPower");
        DataFileReader reader = new(path);

        // Act
        DeviceReading reading = reader.ReadNext(DeviceId);

        // Assert
        Assert.AreEqual(new DateTime(2026, 11, 16, 19, 0, 5), reading.Timestamp);
        Assert.AreEqual(21.5, reading.Value);
        Assert.AreEqual("C", reading.Unit);
        Assert.AreEqual(DeviceStatus.NoPower, reading.Status);
    }

    [TestMethod]
    public void EndOfFileLoopsBackToLineOne()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(DataFileTestHelper.ValidLines(10));
        DataFileReader reader = new(path);

        DeviceReading first = reader.ReadNext(DeviceId);
        for (int i = 1; i < 10; i++)
        {
            reader.ReadNext(DeviceId);
        }

        // Act
        DeviceReading eleventh = reader.ReadNext(DeviceId);
        DeviceReading twelfth = reader.ReadNext(DeviceId);

        // Assert
        Assert.AreEqual(first, eleventh);
        Assert.AreEqual(2.5, twelfth.Value);
    }

    [TestMethod]
    public void BlankAndCommentLinesAreSkipped()
    {
        // Arrange
        string path = DataFileTestHelper.WriteFile(
            "# volume samples",
            "2026-11-16T19:00:00,1,%,Ok",
            "",
            "2026-11-16T19:00:01,2,%,Ok"
        );
        DataFileReader reader = new(path);

        // Act
        double[] values =
        [
            reader.ReadNext(DeviceId).Value,
            reader.ReadNext(DeviceId).Value,
            reader.ReadNext(DeviceId).Value,
        ];

        // Assert
        CollectionAssert.AreEqual(new double[] { 1, 2, 1 }, values);
    }

    [TestMethod]
    public void MissingFileThrowsDataFileException()
    {
        // Arrange
        DataFileReader reader = new(DataFileTestHelper.NewPath());

        // Act + Assert
        Assert.ThrowsExactly<DataFileException>(() => reader.ReadNext(DeviceId));
    }

    [TestMethod]
    public void EmptyFileThrowsDataFileException()
    {
        // Arrange
        DataFileReader reader = new(DataFileTestHelper.WriteFile());

        // Act + Assert
        Assert.ThrowsExactly<DataFileException>(() => reader.ReadNext(DeviceId));
    }

    [TestMethod]
    [DataRow("2026-11-16T19:00:00,40.0,%")]            // too few fields
    [DataRow("2026-11-16T19:00:00,40.0,%,Ok,extra")]   // too many fields
    [DataRow("yesterday,40.0,%,Ok")]                   // bad timestamp
    [DataRow("2026-11-16T19:00:00,loud,%,Ok")]         // bad value
    [DataRow("2026-11-16T19:00:00,NaN,%,Ok")]          // not a finite number
    [DataRow("2026-11-16T19:00:00,40.0,%,Broken")]     // unknown status
    [DataRow("2026-11-16T19:00:00,40.0,%,1")]          // status given as a number
    [DataRow("this is not a data line")]
    public void CorruptLineThrowsDataFileException(string corruptLine)
    {
        // Arrange: the corrupt line sits between two valid ones.
        string path = DataFileTestHelper.WriteFile(
            "2026-11-16T19:00:00,1,%,Ok",
            corruptLine,
            "2026-11-16T19:00:02,3,%,Ok"
        );
        DataFileReader reader = new(path);

        // Act + Assert
        Assert.ThrowsExactly<DataFileException>(() => reader.ReadNext(DeviceId));
    }

    [TestMethod]
    public void ReaderRecoversOnceTheFileAppears()
    {
        // Arrange
        string path = DataFileTestHelper.NewPath();
        DataFileReader reader = new(path);
        Assert.ThrowsExactly<DataFileException>(() => reader.ReadNext(DeviceId));

        // Act
        File.WriteAllLines(path, DataFileTestHelper.ValidLines(3));
        DeviceReading reading = reader.ReadNext(DeviceId);

        // Assert
        Assert.AreEqual(1.5, reading.Value);
    }
}
