namespace SCADAHMI_Project;

//The air conditioning unit and its temperature sensor
public sealed class ACControl : TheatreDevice
{
    public ACControl(string deviceId) : base(deviceId) { }

    //Measured room temperature in degrees Celsius
    public double Temperature { get; set; }

    // Temperature the operator has asked for, in degrees Celsius
    public double TargetTemperature { get; set; } = 21;

    // Operating mode, e.g. "Cool", "Heat" or "Off"
    public string Mode { get; set; } = "Off";
}