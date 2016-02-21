using UnityEngine;
using System.Collections;

public static class RandomExtensions
{
    public static double NextDouble(this System.Random rand, double minValue, double maxValue)
    {
        return rand.NextDouble() * (maxValue - minValue) + minValue;
    }
}