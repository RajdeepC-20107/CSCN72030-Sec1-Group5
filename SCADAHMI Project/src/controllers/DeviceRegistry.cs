using System.Linq;

namespace SCADAHMI_Project;

/// <summary>
/// Class for containing all devices that are to be subscribed and
/// unsubscribed as well as modified.
/// </summary>
public class DeviceRegistry
{
    /// <summary>
    /// The global DeviceRegistry singleton.
    /// </summary>
    public static DeviceRegistry Instance { get; } = new();

    /// <summary>
    /// The container for all devices.
    /// </summary>
    private readonly Dictionary<Guid, Device> devices = [];
    
    /// <summary>
    /// Subscribe a device to be affected by commands.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <param name="instance"></param>
    /// <returns>Whether the subscribe succeeds or not.</returns>
    public bool Subscribe<TDevice>(TDevice instance)
        where TDevice : Device
    {
        return devices.TryAdd(instance.Guid, instance);
    }

    /// <summary>
    /// Unsubscribe a device to no longer be affected by commands.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <param name="instance"></param>
    /// <returns>Whether the unsubscribe succeeds or not.</returns>
    public bool Unsubscribe<TDevice>(TDevice instance)
        where TDevice : Device
    {
        return devices.Remove(instance.Guid);
    }

    /// <summary>
    /// Get all subscribed devices of a certain type.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <returns>A read-only list of that certain device type.</returns>
    public IReadOnlyList<TDevice> GetSubscribedDevices<TDevice>()
        where TDevice : Device
    {
        return devices.Values
            .OfType<TDevice>()
            .ToList();
    }
}