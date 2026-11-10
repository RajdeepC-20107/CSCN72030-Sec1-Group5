using SCADAHMI_Project;

DeviceListener deviceListener = new();

HelloDevice helloDevice1 = new();
HelloDevice helloDevice2 = new();

deviceListener.Subscribe(helloDevice1);
deviceListener.Subscribe(helloDevice2);

Console.WriteLine(
    string.Join(", ", deviceListener.GetSubscribedDevices<HelloDevice>())
);

deviceListener.Unsubscribe(helloDevice1);
deviceListener.Unsubscribe(helloDevice2);
deviceListener.Unsubscribe(helloDevice1);

Console.WriteLine(
    string.Join(", ", deviceListener.GetSubscribedDevices<HelloDevice>())
);