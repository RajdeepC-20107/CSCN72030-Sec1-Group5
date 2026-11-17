using System.Data;
using SCADAHMI_Project;

class CmdMessageBus
{
    Dictionary<Guid, Device> devices = new Dictionary<Guid, Device>();
    private Thread runLoop;
    private Queue<Action> cmdQueue = new();
    
    public void Enqueue<TArgs>(
    IDeviceCommand<TArgs> command,
    TArgs commandArgs)
    where TArgs : CommandArgs
    {
        cmdQueue.Enqueue(() =>
        {
            var matchingDevices = devices.Values
                .Where(d => command.DeviceGUIDs.Contains(d.GUID.ToString()))
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

    private void ExecuteRunLoop()
    {
        while (cmdQueue.Count != 0)
        {
            Console.WriteLine("Executing command");
            Action cmd = cmdQueue.Dequeue();
            cmd.Invoke();
        }
    }
}