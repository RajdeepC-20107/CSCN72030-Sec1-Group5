using System;

namespace SCADAHMI_Project;

/// <summary>
/// Makes managing different Lights commands/actions
/// easy by providing methods for them.
/// </summary>
public class LightsManager : IDeviceManager<Lights, CreateLightsArgs>
{
    private LightsManager() {}

    public static Lights? CreateDevice(CreateLightsArgs args, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        Lights device = new();
        device.Red = args.Red ?? device.Red;
        device.Green = args.Green ?? device.Green;
        device.Blue = args.Blue ?? device.Blue;
        device.MinimumBrightness = args.MinimumBrightness ?? device.MinimumBrightness;
        device.MaximumBrightness = args.MaximumBrightness ?? device.MaximumBrightness;
        bool success = registry.Subscribe(device);
        return (success) ? device : null;
    }

    public static Lights? GetDevice(Guid guid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.GetDeviceOfType<Lights>(guid);
    }

    public static bool RemoveDevice(Lights device, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        return registry.Unsubscribe(device);
    }

    public static bool RemoveDevice(Guid deviceGuid, DeviceRegistry? registry = null)
    {
        registry ??= DeviceRegistry.Instance;
        Lights? device = GetDevice(deviceGuid, registry);
        if(device is null)
        {
            return false;
        }

        return registry.Unsubscribe(device);
    }

    public static void SetLightsRGB(
        List<Lights> lights, 
        byte? red, 
        byte? green, 
        byte? blue,
        DeviceRegistry? registry = null
    )
    {
        registry ??= DeviceRegistry.Instance;
        CommandBus.Publish(
            new SetLightsRGBCommand(),
            new SetLightsRGBCommandArgs(lights, red, green, blue),
            registry
        );
    }
}
