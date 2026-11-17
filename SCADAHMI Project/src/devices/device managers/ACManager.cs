namespace SCADAHMI_Project;

/*
The air conditioning unit and its temperature sensor.
Data file value: room temperature in degrees Celsius.
*/
public sealed class ACManager : DeviceManager
{
    public ACManager(string deviceId, DataFileReader reader)
        : base(deviceId, reader) { }

    // Measured room temperature in degrees Celsius.
    public double Temperature { get; set; }

    // Temperature the operator has asked for, in degrees Celsius.
    public double TargetTemperature { get; set; } = 21;

    // Operating mode, e.g. "Cool", "Heat" or "Off".
    public string Mode { get; set; } = "Off";

    protected override void OnSample(DeviceReading reading)
    {
        Temperature = reading.Value;
    }
}
