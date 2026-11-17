namespace SCADAHMI_Project;

/*
The dimmable room lights.
Data file value: light level in percent (0 = off, 100 = full brightness).
*/
public sealed class LightsManager : DeviceManager
{
    public LightsManager(string deviceId, DataFileReader reader)
        : base(deviceId, reader) { }

    // Light level in percent (0 = off, 100 = full brightness).
    public double DimLevel { get; set; } = 100;

    protected override void OnSample(DeviceReading reading)
    {
        DimLevel = reading.Value;
    }
}
