using SCADAHMI_Project;

DeviceRegistry deviceRegistry = new();

HelloDevice helloDevice1 = new();
HelloDevice helloDevice2 = new();

deviceRegistry.Subscribe(helloDevice1);
deviceRegistry.Subscribe(helloDevice2);
deviceRegistry.Subscribe(helloDevice1);

Console.WriteLine(
    string.Join(", ", deviceRegistry.GetSubscribedDevices<HelloDevice>())
);

deviceRegistry.Unsubscribe(helloDevice1);
deviceRegistry.Unsubscribe(helloDevice2);
deviceRegistry.Unsubscribe(helloDevice1);

Console.WriteLine(
    string.Join(", ", deviceRegistry.GetSubscribedDevices<HelloDevice>())
);

deviceRegistry.Subscribe(helloDevice1);
deviceRegistry.Subscribe(helloDevice2);

CommandBus.Publish(
    deviceRegistry,
    new SetHelloDeviceCommand(),
    new SetHelloDeviceCommandArgs("No way!")
);

