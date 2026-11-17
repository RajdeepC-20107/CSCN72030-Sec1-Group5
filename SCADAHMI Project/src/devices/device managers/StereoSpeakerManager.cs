namespace SCADAHMI_Project;


/* One speaker group (front left, center, surround or rear).
 Create one instance per group, each with its own id.
*/
public sealed class StereoSpeakerManager : TheatreDevice
{
    public StereoSpeakerManager(string deviceId) : base(deviceId) { }

    // Output volume in percent (0-100)
    public double Volume { get; set; }

    // Highest volume the operator allows, in percent (0-100)
    public double MaxVolume { get; set; } = 100;

    // Left/right balance, from -1 (left) to 1 (right)
    public double Balance { get; set; }

    // Gain of each equalizer band in dB
    public double[] EqualizationBands { get; set; } = [0, 0, 0, 0, 0];
}