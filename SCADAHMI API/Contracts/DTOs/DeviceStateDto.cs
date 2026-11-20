namespace SCADAHMI_API.Contracts.DTOs;

/// <summary>
/// Wire representation of a single device.
/// </summary>
public sealed record DeviceStateDto(
    Guid Id,
    string Type,
    string Message
);