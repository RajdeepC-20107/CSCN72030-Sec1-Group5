using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.DTOs;

/// <summary>
/// An abstract record to make data transfer objects from.
/// </summary>
/// <param name="Guid"></param>
/// <param name="Name"></param>
[JsonDerivedType(typeof(HelloDeviceStateDto), "helloDevice")]
[JsonDerivedType(typeof(LightsStateDto), "lights")]
public abstract record DeviceStateDto(
    Guid Guid,
    string Name
);