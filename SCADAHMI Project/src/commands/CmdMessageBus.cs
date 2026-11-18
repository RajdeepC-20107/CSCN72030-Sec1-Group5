using System.Data;
using SCADAHMI_Project;

class CmdMessageBus
{
    Dictionary<Guid, Device> devices = new Dictionary<Guid, Device>();
    private Thread runLoop;
    private Queue<Action> cmdQueue = new();

    // Singleton pattern to ensure only one instance of CmdMessageBus exists
    private static readonly Lazy<CmdMessageBus> _lazy = 
        new(() => new CmdMessageBus());

    public static CmdMessageBus Instance => _lazy.Value;
    
    public void Enqueue<TArgs>(
        IDeviceCommand<TArgs> command,
        TArgs commandArgs
    )
        where TArgs : CommandArgs
    {
        cmdQueue.Enqueue(() =>
        {
            var matchingDevices = devices.Values
                .Where(d => command.DeviceGUIDs == null || command.DeviceGUIDs.Contains(d.GUID.ToString()))
                .Where(d => command.DeviceTypes == null || command.DeviceTypes.Contains(d.Type))
                .ToHashSet();

            command.Execute(commandArgs, matchingDevices);
        });

        if (runLoop == null || !runLoop.IsAlive) {
            runLoop = new Thread(ExecuteRunLoop);
            runLoop.Start();
        }
    }

    public void AddDevice(Device device)
    {
        devices.Add(device.GUID, device);
    }

    public void RemoveDevice(Guid deviceGUID)
    {
        devices.Remove(deviceGUID);
    }

    private void ExecuteRunLoop()
    {
        while (cmdQueue.Count != 0)
        {
            Action cmd = cmdQueue.Dequeue();
            cmd.Invoke();
        }
    }
}