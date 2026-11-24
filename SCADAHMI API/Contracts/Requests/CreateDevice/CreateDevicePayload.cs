using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

/// <summary>
/// Abstract record representing a payload for device creation requests.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CreateHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(CreateLightsPayload), "lights")]
public abstract record CreateDevicePayload;