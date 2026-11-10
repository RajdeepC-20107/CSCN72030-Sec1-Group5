using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace SCADAHMI_Project;

public class DeviceListener
{
    private readonly Dictionary<Type, HashSet<Device>> messageBus = new();
    
    public void Subscribe<T>(T instance)
        where T : Device
    {
        if(!messageBus.ContainsKey(typeof(T))) 
        {
            messageBus.TryAdd(typeof(T), []);
        }

        messageBus[typeof(T)].Add(instance);
    }

    public void Unsubscribe<T>(T instance)
        where T : Device
    {
        if(!messageBus.ContainsKey(typeof(T)))
        {
            return;
        }

        messageBus[typeof(T)].Remove(instance);
    }

    public IReadOnlyList<Device> GetSubscribedDevices<T>()
        where T : Device
    {
        if(!messageBus.ContainsKey(typeof(T)))
        {
            messageBus.TryAdd(typeof(T), []);
        }

        return messageBus[typeof(T)].ToList();
    }
}
