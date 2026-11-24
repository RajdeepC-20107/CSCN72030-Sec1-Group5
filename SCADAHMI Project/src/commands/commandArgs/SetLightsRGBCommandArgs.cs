namespace SCADAHMI_Project;

/// <summary>
/// Command arguments for setting the RGB of multiple lights.
/// </summary>
public record SetLightsRGBCommandArgs(
    List<Lights> Lights,
    byte? Red,
    byte? Green,
    byte? Blue
) : CommandArgs;