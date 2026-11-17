namespace SCADAHMI_Project;

/*
The seat motion sensor. It is monitor-only: nothing on it is controlled.
Data file value: seat occupancy as a bit mask, where bit 0 is seat 1,
bit 1 is seat 2 and so on. For example 5 (binary 101) means seats 1
and 3 are occupied.
*/
public sealed class MotionSensorManager : DeviceManager
{
    public MotionSensorManager(string deviceId, DataFileReader reader)
        : base(deviceId, reader) { }

    // Seat numbers (starting at 1) that are currently occupied (FR-004).
    public IReadOnlyList<int> OccupiedSeats { get; set; } = [];

    protected override void OnSample(DeviceReading reading)
    {
        int mask = (int)reading.Value;
        List<int> seats = [];

        for (int seat = 1; mask != 0; seat++, mask >>= 1)
        {
            if ((mask & 1) == 1)
            {
                seats.Add(seat);
            }
        }

        OccupiedSeats = seats;
    }
}
