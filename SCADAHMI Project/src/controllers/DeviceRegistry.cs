using System.Collections.Concurrent;
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
    private readonly ConcurrentDictionary<Guid, Device> devices = new();

    private int version;

    /// <summary>
    /// Raised whenever the registry revision advances. Any mutation that
    /// calls <see cref="MarkChanged"/> automatically notifies listeners,
    /// regardless of which command or manager performed it.
    /// </summary>
    public event EventHandler<int>? Changed;

    /// <summary>
    /// Monotonic revision of the registry; clients use it to detect changes.
    /// </summary>
    public int Version => Volatile.Read(ref version);

    /// <summary>
    /// Bumps the revision after a device or its state has changed.
    /// </summary>
    public void MarkChanged()
    {
        int next = Interlocked.Increment(ref version);
        Changed?.Invoke(this, next);
    }
    
    /// <summary>
    /// Subscribe a device to be affected by commands.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <param name="instance"></param>
    /// <returns>Whether the subscribe succeeds or not.</returns>
    public bool Subscribe<TDevice>(TDevice instance)
        where TDevice : Device
    {
        if (!devices.TryAdd(instance.Guid, instance))
        {
            return false;
        }

        MarkChanged();
        return true;
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
        if (!devices.TryRemove(instance.Guid, out _))
        {
            return false;
        }

        MarkChanged();
        return true;
    }

    /// <summary>
    /// Get a subscribed device based on its GUID.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns>The device if found, null if not.</returns>
    public Device? GetDevice(Guid guid)
    {
        devices.TryGetValue(guid, out Device? device);
        return device;
    }

    /// <summary>
    /// Get a subscribed device based on its GUID if the device type matches.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <param name="guid"></param>
    /// <returns>The device if found and the type matches, null if not.</returns>
    public TDevice? GetDeviceOfType<TDevice>(Guid guid)
        where TDevice : Device
    {
        devices.TryGetValue(guid, out Device? device);
        if(device is null)
        {
            return null;
        }

        return device is TDevice typed 
            ? typed 
            : null;
    }

    /// <summary>
    /// Get all subscribed devices of a certain type.
    /// </summary>
    /// <typeparam name="TDevice"></typeparam>
    /// <returns>A read-only list of that certain device type.</returns>
    public IReadOnlyList<TDevice> GetDevicesOfType<TDevice>()
        where TDevice : Device
    {
        return devices.Values
            .OfType<TDevice>()
            .ToList();
    }
}