using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CreateHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(CreateLightsPayload), "lights")]
public abstract record CreateDevicePayload;