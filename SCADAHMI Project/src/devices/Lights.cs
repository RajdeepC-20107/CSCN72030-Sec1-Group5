using System;

namespace SCADAHMI_Project;

public class Lights : Device
{
    public override string Name { get; set; } = nameof(Lights);
    
    public bool IsDimmed { get; set; } = false;
    private static readonly ushort MIN_BRIGHTNESS = 0;
    private static readonly ushort MAX_BRIGHTNESS = 100;
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

    public ushort MaximumBrightness { 
        get; set
        {
            field = Math.Clamp(value, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
            if(MinimumBrightness > field) {
                MinimumBrightness = field;
            }
        } 
    } = 100;

    public byte Red { get; set; } = 255;
    public byte Green { get; set; } = 255;
    public byte Blue { get; set; } = 255;
}
