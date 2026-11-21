namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

public sealed record CreateHelloDevicePayload(
    string? Message
) : CreateDevicePayload;
