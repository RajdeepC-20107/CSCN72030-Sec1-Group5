using System;

namespace SCADAHMI_Project;

/// <summary>
/// Makes managing different HelloDevice commands/actions
/// easy by providing methods for them.
/// </summary>
public class HelloDeviceManager : IDeviceManager<HelloDevice, CreateHelloDeviceArgs>
{
    private HelloDeviceManager() {}

    public static HelloDevice? CreateDevice(CreateHelloDeviceArgs args, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        HelloDevice device = new();
        device.Message = args.Message ?? device.Message;
        bool success = registry.Subscribe(device);
        return (success) ? device : null;
    }

    public static HelloDevice? GetDevice(Guid guid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.GetDeviceOfType<HelloDevice>(guid);
    }

    public static bool RemoveDevice(HelloDevice device, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.Unsubscribe(device);
    }

    public static bool RemoveDevice(Guid deviceGuid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        HelloDevice? device = GetDevice(deviceGuid, registry);
        if(device is null)
        {
            return false;
        }

        return registry.Unsubscribe(device);
    }

    /// <summary>
    /// Sets the messages of the given HelloDevice list.
    /// </summary>
    /// <param name="deviceMessages"></param>
    /// <param name="registry"></param>
    public static void SetHelloDeviceMessages(
        List<(HelloDevice, string)> deviceMessages,
        DeviceRegistry? registry = null
    )
    {
        registry ??= DeviceRegistry.Instance;
        var devices = registry.GetDevicesOfType<HelloDevice>()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(var (device, message) in deviceMessages)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)) {
                subscribed.Message = message;
                changed = true;
            }
        }

        if (changed)
        {
            registry.MarkChanged();
        }
    }
}
