namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

public sealed record HelloDevicePayload(
    string? Message
) : CreateDevicePayload;
