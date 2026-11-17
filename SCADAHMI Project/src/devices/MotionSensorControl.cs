namespace SCADAHMI_Project;


// The seat motion sensor. It is monitor-only: nothing on it is controlled.

public sealed class MotionSensorControl : TheatreDevice
{
    public MotionSensorControl(string deviceId) : base(deviceId) { }

    // Seat numbers (starting at 1) that are currently occupied (FR-004).
    public IReadOnlyList<int> OccupiedSeats {get; set;} = [];
}