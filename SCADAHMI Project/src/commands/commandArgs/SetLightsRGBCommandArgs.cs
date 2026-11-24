namespace SCADAHMI_Project;

/// <summary>
/// Record describing the way to change the RGB of multiple lights.
/// </summary>
public record SetLightsRGBCommandArgs(
    List<Lights> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
) : CommandArgs;