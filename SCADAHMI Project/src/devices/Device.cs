using System;

namespace SCADAHMI_Project;

/// <summary>
/// Abstract class to make devices from.
/// </summary>
public abstract class Device
{
    public Guid Guid { get; } = Guid.NewGuid();
}
