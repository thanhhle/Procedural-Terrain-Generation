using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffMapGenerator
{
   public static float[,] GenerateFalloffMap(int size)
   {
        float[,] falloffMap = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float X = x / (float)size * 2 - 1;
                float Y = y / (float)size * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(X), Mathf.Abs(Y));
                falloffMap[x, y] = Evaluate(value);
            }
        }

        return falloffMap;
   }


    private static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
