namespace SCADAHMI_Project;

/*
Common base for every device in the theatre. It extends the existing
Device class, so these devices work with DeviceListener
and the command classes exactly like HelloDevice does.

It also owns the device's looped data file: Read returns
the next sample, and a missing or corrupt file becomes a Fault reading
instead of an exception (NFR-006).
*/
public abstract class DeviceManager : Device, IDevice
{
    private readonly DataFileReader reader;

    /*
    deviceId: Unique id of this device, e.g. "speaker-front-left".
    reader: Reader over this device's own data file.
    */
    protected DeviceManager(string deviceId, DataFileReader reader)
    {
        DeviceId = deviceId;
        this.reader = reader;
    }

    // Unique id of this device instance.
    public string DeviceId { get; }

    // Status of the most recent reading. Ok before the first poll.
    public DeviceStatus Status { get; private set; } = DeviceStatus.Ok;

    /*
    Reads the next line of the data file. Never throws: a file problem
    is reported as a reading with status Fault, and the file is tried
    again on the next poll.
    */
    public DeviceReading Read()
    {
        DeviceReading reading;

        try
        {
            reading = reader.ReadNext(DeviceId);
        }
        catch (DataFileException e)
        {
            reader.Reset();
            reading = DeviceReading.Fault(DeviceId, e.Message);
        }

        Status = reading.Status;

        if (reading.Status == DeviceStatus.Ok)
        {
            OnSample(reading);
        }

        return reading;
    }

    /*
    Called with every Ok reading so the device can copy the sampled
    value into its own property (volume, temperature, ...).
    */
    protected abstract void OnSample(DeviceReading reading);

    public override string ToString() => DeviceId;
}
