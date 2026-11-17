namespace SCADAHMI_Project;

/*
The projector.
Data file value: lamp brightness in percent (0-100).
*/
public sealed class ProjectorManager : DeviceManager
{
    public ProjectorManager(string deviceId, DataFileReader reader)
        : base(deviceId, reader) { }

    // Whether the projector is switched on (FR-003).
    public bool PowerOn { get; set; }

    // Lamp brightness in percent (0-100).
    public double Brightness { get; set; }

    // Lens focus (0-100).
    public double Focus { get; set; } = 50;

    // Horizontal keystone correction in degrees.
    public double KeystoneHorizontal { get; set; }

    // Vertical keystone correction in degrees.
    public double KeystoneVertical { get; set; }

    protected override void OnSample(DeviceReading reading)
    {
        Brightness = reading.Value;
    }
}
