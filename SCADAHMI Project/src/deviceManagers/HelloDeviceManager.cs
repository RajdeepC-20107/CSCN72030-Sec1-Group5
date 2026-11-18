using System;
using System.Linq;

namespace SCADAHMI_Project;

public static class HelloDeviceManager
{
    private static HashSet<HelloDevice> deviceRegistry = [];
    
    public static void CreateDevice(string name)
    {
        HelloDevice helloDevice = new()
        {
            GUID = Guid.NewGuid(),
            Name = name
        };

        deviceRegistry.Add(helloDevice);
        // globalBus.AddDevice(helloDevice)
    }

    public static void RemoveDevice(Guid deviceGUID)
    {
        HelloDevice? device = deviceRegistry.FirstOrDefault(d => d!.GUID == deviceGUID, null);
        if (device is not null) {
            deviceRegistry.Remove(device);
            // globalBus.RemoveDevice(device)
        }
    }

    public static void PrintAllMessages()
    {
        PrintHelloDeviceMessageCommand command = new()
        {
            DeviceGUIDs = (from d in deviceRegistry select d.GUID.ToString()).ToHashSet()
        };

        // globaBus.Enqueue(printCommand, new NoArgs());
    }
}
