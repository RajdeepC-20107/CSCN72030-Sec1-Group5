namespace SCADAHMI_Project;

//The dimmable room lights
public sealed class LightsControl : TheatreDevice
{
    public LightsControl(string deviceId) : base(deviceId) { }

    //Light level in percent (0 = off, 100 = full brightness)
    public double DimLevel {get; set;} = 100;
}