using SCADAHMI_Project.src.utils;

namespace SCADAHMI_Project;

public class DeviceListener
{
    private readonly Dictionary<Type, OrderedHashSet<Device>> devices = new();
    
    public void Subscribe<T>(T instance)
        where T : Device
    {
        if(!devices.ContainsKey(typeof(T))) 
        {
            devices.TryAdd(typeof(T), []);
        }

        devices[typeof(T)].Add(instance);
    }

    public void Unsubscribe<T>(T instance)
        where T : Device
    {
        if(!devices.ContainsKey(typeof(T)))
        {
            return;
        }

        devices[typeof(T)].Remove(instance);
    }

    public IReadOnlyList<T> GetSubscribedDevices<T>()
        where T : Device
    {
        if(!devices.ContainsKey(typeof(T)))
        {
            return [];
        }

        return devices[typeof(T)].Items.Cast<T>().ToList();
    }
}