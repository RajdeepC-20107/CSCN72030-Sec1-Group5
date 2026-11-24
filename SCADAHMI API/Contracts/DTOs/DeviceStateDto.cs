using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.DTOs;

[JsonDerivedType(typeof(HelloDeviceStateDto), "helloDevice")]
[JsonDerivedType(typeof(LightsStateDto), "lights")]
public abstract record DeviceStateDto(
    Guid Guid,
    string Name
);