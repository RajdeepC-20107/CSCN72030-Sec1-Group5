using System;

namespace SCADAHMI_Project;

/// <summary>
/// Abstract class to make devices from.
/// </summary>
public abstract class Device
{
    /// <summary>
    /// The unique identifier of the device.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    /// The name given to the device. It is expected to give this
    /// a default value per class being nameof({device class})
    /// </summary>
    public abstract string Name { get; set; }

    /// <summary>
    /// Whether the device is powered on or not.
    /// </summary>
    public bool PoweredOn { get; set; } = true;
}
