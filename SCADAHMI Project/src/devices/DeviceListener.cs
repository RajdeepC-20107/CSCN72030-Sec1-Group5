using SCADAHMI_Project.src.utils;

namespace SCADAHMI_Project;

/*
Additional implementation to the DeviceListener starts here:

Every subscribed device that implements
IDevice is polled at a fixed interval, and each DeviceReading is passed
to every reading handler. A device that cannot be read is
reported with status Fault and the others carry on.
*/
public class DeviceListener : IDisposable
{
    private readonly object gate = new();

    private readonly Dictionary<Type, OrderedHashSet<Device>> devices = new();

    // Every subscribed device that can be polled, in subscription order.
    private readonly OrderedHashSet<IDevice> pollableDevices = new();

    // The functions that receive each reading (the Hub subscribes here).
    private readonly List<Action<DeviceReading>> readingHandlers = [];

    private CancellationTokenSource? cancellation;
    private Task? loop;

    // Whether the fixed-interval polling loop is running.
    public bool IsRunning => loop is not null;

    public void Subscribe<T>(T instance)
        where T : Device
    {
        lock (gate)
        {
            if(!devices.ContainsKey(typeof(T)))
            {
                devices.TryAdd(typeof(T), []);
            }

            devices[typeof(T)].Add(instance);

            // Devices such as HelloDevice have no Read() and are not polled.
            if(instance is IDevice pollable)
            {
                pollableDevices.Add(pollable);
            }
        }
    }

    public void Unsubscribe<T>(T instance)
        where T : Device
    {
        lock (gate)
        {
            if(!devices.ContainsKey(typeof(T)))
            {
                return;
            }

            devices[typeof(T)].Remove(instance);

            /*
            Stop polling the device only once it is no longer
            subscribed under any type.
            */
            if(instance is IDevice pollable
                && !devices.Values.Any(set => set.Contains(instance)))
            {
                pollableDevices.Remove(pollable);
            }
        }
    }

    public IReadOnlyList<T> GetSubscribedDevices<T>()
        where T : Device
    {
        lock (gate)
        {
            if(!devices.ContainsKey(typeof(T)))
            {
                return [];
            }

            return devices[typeof(T)].Items.Cast<T>().ToList();
        }
    }

    /*
    Registers a function that receives every reading. Once Start has
    been called the function runs on a background thread, so a WPF
    subscriber must switch to the UI thread (Dispatcher.Invoke) before
    touching controls.
    */
    public void SubscribeReadings(Action<DeviceReading> handler)
    {
        lock (gate)
        {
            readingHandlers.Add(handler);
        }
    }

    // Removes a function added with SubscribeReadings.
    public void UnsubscribeReadings(Action<DeviceReading> handler)
    {
        lock (gate)
        {
            readingHandlers.Remove(handler);
        }
    }

    /*
    Passes one reading to every reading handler. A handler that throws
    does not stop the other handlers.
    */
    public void PublishReading(DeviceReading reading)
    {
        Action<DeviceReading>[] handlersNow;

        lock (gate)
        {
            handlersNow = readingHandlers.ToArray();
        }

        foreach(Action<DeviceReading> handler in handlersNow)
        {
            try
            {
                handler(reading);
            }
            catch (Exception)
            {
                // A failing handler must not block the others.
                // TODO: report this through the EventLogger once it exists.
            }
        }
    }

    /*
    Polls every pollable device once, in subscription order, and
    publishes one reading per device. Returns the readings that were
    published. A device that throws is reported as Fault and does not
    stop the others.
    */
    public IReadOnlyList<DeviceReading> PollOnce()
    {
        IDevice[] devicesNow;

        // Copy under the lock so a handler can subscribe devices freely.
        lock (gate)
        {
            devicesNow = pollableDevices.Items.ToArray();
        }

        List<DeviceReading> readings = [];

        foreach(IDevice device in devicesNow)
        {
            DeviceReading reading;

            try
            {
                reading = device.Read();
            }
            catch (Exception e)
            {
                /*
                IDevice.Read should not throw, but one bad device must
                never take the others down with it.
                */
                reading = DeviceReading.Fault(device.DeviceId, e.Message);
            }

            readings.Add(reading);
            PublishReading(reading);
        }

        return readings;
    }

    /*
    Starts polling every interval on a background thread.
    Calling it while already running has no effect.
    */
    public void Start(TimeSpan interval)
    {
        if(interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be positive.");
        }

        if(loop is not null)
        {
            return;
        }

        cancellation = new CancellationTokenSource();
        loop = RunAsync(interval, cancellation.Token);
    }

    // Stops the polling loop and waits for the current poll to finish.
    public void Stop()
    {
        if(loop is null)
        {
            return;
        }

        cancellation!.Cancel();

        try
        {
            loop.Wait();
        }
        catch (AggregateException)
        {
            // The loop ends by cancellation; nothing to report.
        }

        cancellation.Dispose();
        cancellation = null;
        loop = null;
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private async Task RunAsync(TimeSpan interval, CancellationToken token)
    {
        // PeriodicTimer never overlaps ticks, so a slow poll cannot pile up.
        using PeriodicTimer timer = new(interval);

        try
        {
            // Task.Run keeps a slow poll off the caller's (UI) thread.
            while(await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                await Task.Run(PollOnce, token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Stop() was called.
        }
    }
}
