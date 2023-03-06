using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = NoiseMapGenerator.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        AnimationCurve _heightCurve = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                values[x, y] *= _heightCurve.Evaluate(values[x, y]) * settings.heightMultiplier;

                maxValue = values[x, y] > maxValue ? values[x, y] : maxValue;
                minValue = values[x, y] < minValue ? values[x, y] : minValue;
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}



public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

