using System;

namespace SCADAHMI_Project;

public class CalculatorDevice : Device
{
    public int Value1 {get; protected set;} = 10;

    public int Value2 {get; protected set;} = 20;

    public int Add() => Value1 + Value2;

    public void SetValues(int? value1, int? value2)
    {
        Value1 = value1 ?? Value1;
        Value2 = value2 ?? Value2;
    }

    public int GetSum() => Value1 + Value2;
}
