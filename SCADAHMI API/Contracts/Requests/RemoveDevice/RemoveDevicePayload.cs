using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

/// <summary>
/// Abstract record representing a payload for device removal requests.
/// It is currently made under the assumption multiple device removals
/// cannot happen at once.
/// </summary>
/// <param name="Guid"></param>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RemoveHelloDevicePayload), "helloDevice")]
[JsonDerivedType(typeof(RemoveLightsPayload), "lights")]
public abstract record RemoveDevicePayload(
    Guid Guid
);