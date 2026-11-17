namespace SCADAHMI_Project;

/*
One speaker group (front left, center, surround or rear).
Create one instance per group, each with its own id and data file.
Data file value: output volume in percent (0-100).
*/
public sealed class StereoSpeakerManager : DeviceManager
{
    public StereoSpeakerManager(string deviceId, DataFileReader reader)
        : base(deviceId, reader) { }

    // Output volume in percent (0-100).
    public double Volume { get; set; }

    // Highest volume the operator allows, in percent (FR-006).
    public double MaxVolume { get; set; } = 100;

    // Left/right balance, from -1 (left) to 1 (right).
    public double Balance { get; set; }

    // Gain of each equalizer band in dB.
    public double[] EqualizationBands { get; set; } = [0, 0, 0, 0, 0];

    protected override void OnSample(DeviceReading reading)
    {
        // The speaker is never driven above the operator's maximum.
        Volume = Math.Min(reading.Value, MaxVolume);
    }
}
