using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float InchesToMeters(this float inches)
    {
        return inches / 39.3701f;
    }
    public static float MetersToInches(this float meters)
    {
        return meters * 39.3701f;
    }
}
