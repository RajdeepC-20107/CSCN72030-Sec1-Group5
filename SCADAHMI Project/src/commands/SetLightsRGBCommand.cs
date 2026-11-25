using System;

namespace SCADAHMI_Project;

/// <summary>
/// Command to set the specified Lights' RGB values.
/// </summary>
public sealed class SetLightsRGBCommand
    : IDeviceCommand<SetLightsRGBCommandArgs>
{
    bool IDeviceCommand<SetLightsRGBCommandArgs>.Execute(
        DeviceRegistry deviceRegistry, 
        SetLightsRGBCommandArgs args
    ) {
        if(args.Red is null && args.Green is null && args.Blue is null)
        {
            return false;
        }

        var devices = deviceRegistry.GetDevicesOfType<Lights>()
            .ToDictionary(d => d.Guid);
        
        bool changed = false;

        foreach(Lights device in args.Lights)
        {
            if (devices.TryGetValue(device.Guid, out var subscribed)) 
            {
                if(args.Red is not null && subscribed.Red != args.Red)
                {
                    subscribed.Red = (byte)args.Red;
                    changed = true;
                }
                if(args.Green is not null && subscribed.Green != args.Green)
                {
                    subscribed.Green = (byte)args.Green;
                    changed = true;
                }
                if(args.Blue is not null && subscribed.Blue != args.Blue)
                {
                    subscribed.Blue = (byte)args.Blue;
                    changed = true;
                }
            }
        }

        return changed;
    }
}
