using System;
using System.Linq;

namespace SCADAHMI_Project;

public class DeviceFactory<TDevice>
    where TDevice : Device, new()
{    
    public static void CreateDevice(string name)
    {
        TDevice device = new()
        {
            GUID = Guid.NewGuid(),
            Name = name
        };

        CmdMessageBus.Instance.AddDevice(device);
    }

    public static void RemoveDevice(Guid deviceGUID)
    {
        CmdMessageBus.Instance.RemoveDevice(deviceGUID);
    }

    // public static void PrintAllMessages()
    // {
    //     PrintHelloDeviceMessageCommand command = new()
    //     {
    //         DeviceGUIDs = (from d in deviceRegistry select d.GUID.ToString()).ToHashSet()
    //     };

    //     CmdMessageBus.Instance.Enqueue(command, new NoArgs());
    // }
}
