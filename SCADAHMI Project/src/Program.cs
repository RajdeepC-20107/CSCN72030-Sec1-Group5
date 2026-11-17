using SCADAHMI_Project;

DeviceListener deviceListener = new();

HelloDevice helloDevice1 = new();
HelloDevice helloDevice2 = new();

deviceListener.Subscribe(helloDevice1);
deviceListener.Subscribe(helloDevice2);
deviceListener.Subscribe(helloDevice1);

Console.WriteLine(
    string.Join(", ", deviceListener.GetSubscribedDevices<HelloDevice>())
);

deviceListener.Unsubscribe(helloDevice1);
deviceListener.Unsubscribe(helloDevice2);
deviceListener.Unsubscribe(helloDevice1);

Console.WriteLine(
    string.Join(", ", deviceListener.GetSubscribedDevices<HelloDevice>())
);

deviceListener.Subscribe(helloDevice1);
deviceListener.Subscribe(helloDevice2);

CommandHandler.Publish(
    deviceListener,
    new SetHelloDeviceCommand(),
    new SetHelloDeviceCommandArgs("No way!")
);


/*
Looped data file demo: polls every device once a second and prints
each DeviceReading. Each data file has 10 lines, so the values repeat
after 10 seconds. Press Enter to stop.
*/

string dataFolder = Path.Combine(AppContext.BaseDirectory, "data");

DataFileReader FileFor(string name) => new(Path.Combine(dataFolder, name));

// Subscribing a device makes it reachable by commands and polled for readings.
deviceListener.Subscribe(new StereoSpeakerManager("speaker-front-left", FileFor("speaker_front_left.csv")));
deviceListener.Subscribe(new StereoSpeakerManager("speaker-center", FileFor("speaker_center.csv")));
deviceListener.Subscribe(new StereoSpeakerManager("speaker-surround", FileFor("speaker_surround.csv")));
deviceListener.Subscribe(new StereoSpeakerManager("speaker-rear", FileFor("speaker_rear.csv")));
deviceListener.Subscribe(new ProjectorManager("projector", FileFor("projector.csv")));
deviceListener.Subscribe(new LightsManager("lights", FileFor("lights.csv")));
deviceListener.Subscribe(new ACManager("ac", FileFor("ac.csv")));
deviceListener.Subscribe(new MotionSensorManager("motion-sensor", FileFor("motion_sensor.csv")));

int pollNumber = 0;
int devicesPerPoll = 8;
int readingsInThisPoll = 0;

deviceListener.SubscribeReadings(reading =>
{
    if (readingsInThisPoll == 0)
    {
        pollNumber++;
        Console.WriteLine();
        Console.WriteLine($"--- Poll {pollNumber} ---");
    }

    readingsInThisPoll = (readingsInThisPoll + 1) % devicesPerPoll;

    Console.WriteLine(
        $"{reading.Timestamp:HH:mm:ss}  {reading.DeviceId,-20}"
        + $"{reading.Value,7:0.0} {reading.Unit,-6} {reading.Status}"
        + (reading.Detail is null ? "" : $"  ({reading.Detail})")
    );
});

Console.WriteLine();
Console.WriteLine($"Reading data files from: {dataFolder}");
Console.WriteLine("Polling every second. Press Enter to stop.");

deviceListener.Start(TimeSpan.FromSeconds(1));
Console.ReadLine();
deviceListener.Stop();
