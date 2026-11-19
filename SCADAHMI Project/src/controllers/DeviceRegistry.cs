namespace SCADAHMI_Project;

/// <summary>
/// Class for containing all devices that are to be subscribed and
/// unsubscribed as well as modified.
/// </summary>
public class DeviceRegistry
{
    /// <summary>
    /// The container for all devices.
    /// </summary>
    private readonly Dictionary<Type, OrderedHashSet<Device>> devices = [];
    
    /// <summary>
    /// Subscribe a device to be affected by commands.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <param name="instance"></param>
    /// <returns>Whether the subscribe succeeds or not.</returns>
    public bool Subscribe<TDevice>(TDevice instance)
        where TDevice : Device
    {
        if(!devices.ContainsKey(typeof(TDevice))) 
        {
            devices.TryAdd(typeof(TDevice), []);
        }

        return devices[typeof(TDevice)].Add(instance);
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
        if(!devices.ContainsKey(typeof(TDevice)))
        {
            return false;
        }

        return devices[typeof(TDevice)].Remove(instance);
    }

    /// <summary>
    /// Get all subscribed devices of a certain type.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <returns>A read-only list of that certain device type.</returns>
    public IReadOnlyList<TDevice> GetSubscribedDevices<TDevice>()
        where TDevice : Device
    {
        if(!devices.ContainsKey(typeof(TDevice)))
        {
            return [];
        }

        return devices[typeof(TDevice)]
            .Items
            .Cast<TDevice>()
            .ToList();
    }
}