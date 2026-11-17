namespace SCADAHMI_Project;

/*
Anything the Hub can monitor. Adding a new device means adding a class
that implements this interface; the bus and the Hub do not change
*/
public interface IDevice
{
    // Unique id, e.g. "speaker-front-left".
    string DeviceId { get; }

    /*
    Takes one sample. Implementations must not throw: a device that
    cannot produce a sample returns a reading with status Fault.
    */
    DeviceReading Read();
}
