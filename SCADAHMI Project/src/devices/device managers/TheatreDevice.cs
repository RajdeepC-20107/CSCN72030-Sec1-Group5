namespace SCADAHMI_Project;


/* Common base for every device in the theatre. It extends the existing
 Device class, so these devices work with DeviceListener
 and the command classes exactly like HelloDevice does.
*/
public abstract class TheatreDevice : Device
{
    // Unique id of this device, e.g. "speaker-front-left"
    protected TheatreDevice(string deviceId)
    {
        DeviceId = deviceId;
    }

    // Unique id of this device instance
    public string DeviceId { get; }

    public override string ToString() => DeviceId;
}