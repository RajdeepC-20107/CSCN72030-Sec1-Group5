using System.Text.Json.Serialization;

namespace SCADAHMI_API.Contracts.Requests.CreateDevice;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CreateHelloDevicePayload), "helloDevice")]
// TODO: [JsonDerivedType(typeof(CalculatorDevicePayload), "calculatorDevice")]
public abstract record CreateDevicePayload;