namespace SCADAHMI_Project;


/* The defined data structure passed from a device to the Hub and on to
/// the HMI (REQ-SYS-050). One reading corresponds to one line of the
device's data file: <c>Timestamp,Value,Unit,Status</c>.
*/

// DeviceID is Id of the device that produced the reading.</param>
// Timestamp is the timestamp of the sample (from the data file).</param>
// Value is the sampled value. 0 for a Fault reading.</param>
// Unit is the unit of <paramref name="Value"/>, e.g. "%", "C", "seats".</param>
// Status is the health of the device at the time of the sample.</param>
//Detail is a human-readable reason, filled in for Fault readings.</param>
public sealed record DeviceReading(
    string DeviceId,
    DateTime Timestamp,
    double Value,
    string Unit,
    DeviceStatus Status,
    string? Detail = null
)
{
    
    // Builds the reading a device reports when it cannot produce a sample.
   
    public static DeviceReading Fault(string deviceId, string detail) => 
    new(deviceId, DateTime.Now, 0, string.Empty, DeviceStatus.Fault, detail);
}