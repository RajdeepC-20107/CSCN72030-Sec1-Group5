namespace SCADAHMI_Project;


//Health of a device as carried in every <see cref="DeviceReading"/>.
// Fault, NoPower and Disconnected all count toward the fault threshold

public enum DeviceStatus
{
    //The device produced a valid sample.</summary>
    Ok,

    //The data file is missing or corrupt, or the device threw while reading.</summary>
    Fault,

    //The data file says the device has lost power.</summary>
    NoPower,

    //The data file says the device has dropped off the network.</summary>
    Disconnected
}