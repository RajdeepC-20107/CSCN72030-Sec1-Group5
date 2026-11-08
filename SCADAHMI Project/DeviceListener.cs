using System;

namespace SCADAHMI_Project;

public static class DeviceListener
{
    private static Dictionary<Type, HashSet<Device>> messageBus = 
        new Dictionary<Type, HashSet<Device>>();
    
    public static void subscribe<T>(T instance)
        where T : Device
    {
        
    }

    public static void unsubscribe<T>(T instance)
        where T : Device
    {
        
    }
}
