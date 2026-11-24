using System;

namespace SCADAHMI_Project;

/// <summary>
/// Device representing any form of lights.
/// </summary>
public class Lights : Device
{
    public override string Name { get; set; } = nameof(Lights);
    
    /// <summary>
    /// Whether the lights have been dimmed or not.
    /// </summary>
    public bool IsDimmed { get; set; } = false;

    /// <summary>
    /// The minimum brightness a light can be (0%)
    /// </summary>
    private static readonly ushort MIN_BRIGHTNESS = 0;

    /// <summary>
    /// The maximum brightness a light can be (100%)
    /// </summary>
    private static readonly ushort MAX_BRIGHTNESS = 100;

    /// <summary>
    /// The minimum brightness of the light.
    /// </summary>
    public ushort MinimumBrightness
    {
        get;
        set { 
            field = Math.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS); 
            if(MaximumBrightness < field) {
                MaximumBrightness = field;
            }
        }
    } = 10;

    /// <summary>
    /// The maximum brightness of the light
    /// </summary>
    public ushort MaximumBrightness { 
        get; set
        {
            field = Math.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
            if(MinimumBrightness > field) {
                MinimumBrightness = field;
            }
        } 
    } = 100;

    /// <summary>
    /// The red value of the light in RGB.
    /// </summary>
    public byte Red { get; set; } = 255;

    /// <summary>
    /// The green value of the light in RGB.
    /// </summary>
    public byte Green { get; set; } = 255;
    
    /// <summary>
    /// The blue value of the light in RGB.
    /// </summary>
    public byte Blue { get; set; } = 255;
}
