using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RemoveHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(RemoveLightsPayload), "lights")]
public abstract record RemoveDevicePayload(
    Guid Guid
);