using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.RemoveDevice;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(RemoveHelloDevicePayload), "helloDevice")]
// TODO: [JsonDerivedType(typeof(CalculatorDevicePayload), "calculatorDevice")]
public abstract record RemoveDevicePayload(
    Guid Guid
);