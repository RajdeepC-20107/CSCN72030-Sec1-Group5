using System;

namespace SCADAHMI_Project;

/// <summary>
/// Interface that allows for consistent creation of
/// device manager classes. It is assumed all of them will contain
/// only static methods, even if they can't be static classes.
/// </summary>
/// <typeparam name="TDevice"></typeparam>
/// <typeparam name="TCreateDeviceArgs"></typeparam>
public interface IDeviceManager<TDevice, TCreateDeviceArgs>
    where TDevice : Device
    where TCreateDeviceArgs : CreateDeviceArgs
{
    /// <summary>
    /// Creates and subscribes a new device for this device manager's TDevice.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="registry">This must be checked for and defaulted to DeviceRegistry.Instance</param>
    /// <returns>The created instance if added to the device registry. Otherwise, null.</returns>
    abstract static TDevice? CreateDevice(TCreateDeviceArgs args, DeviceRegistry? registry = null);

    /// <summary>
    /// Unsubscribes an existing device for this device manager's TDevice.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="registry">This must be checked for and defaulted to DeviceRegistry.Instance</param>
    /// <returns>True if the removal succeeds, false if not.</returns>
    abstract static bool RemoveDevice(TDevice device, DeviceRegistry? registry = null);

    /// <summary>
    /// Unsubscribes an existing device for this device manager's TDevice.
    /// </summary>
    /// <param name="deviceGuid"></param>
    /// <param name="registry">This must be checked for and defaulted to DeviceRegistry.Instance</param>
    /// <returns>True if the removal succeeds, false if not.</returns>
    abstract static bool RemoveDevice(Guid deviceGuid, DeviceRegistry? registry = null);

    /// <summary>
    /// Gets a device of the device manager's associated type if
    /// the GUID is found and matches the type.
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="registry">This must be checked for and defaulted to DeviceRegistry.Instance</param>
    /// <returns></returns>
    abstract static TDevice? GetDevice(Guid guid, DeviceRegistry? registry = null);
}
