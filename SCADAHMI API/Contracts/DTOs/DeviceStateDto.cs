using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.DTOs;

[JsonDerivedType(typeof(HelloDeviceStateDto), "helloDevice")]
public abstract record DeviceStateDto(
    Guid Guid,
    string Name,
    bool PoweredOn
);