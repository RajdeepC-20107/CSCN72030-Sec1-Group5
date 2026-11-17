namespace SCADAHMI_Project;

//The projector
public sealed class ProjectorControl : TheatreDevice
{
    public ProjectorControl(string deviceId) : base(deviceId) { }

    // Whether the projector is switched on
    public bool PowerOn {get; set;}

    // Lamp brightness in percent (0-100)
    public double Brightness {get; set;}

    // Lens focus (0-100)
    public double Focus {get; set;} = 50;

    // Horizontal keystone correction in degrees
    public double KeystoneHorizontal {get; set;}

    // Vertical keystone correction in degrees
    public double KeystoneVertical {get; set;}
}