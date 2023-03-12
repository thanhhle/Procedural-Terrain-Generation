using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMapGenerator
{
    public enum NormalizedMode {Local, Global}

    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[width, height];

        System.Random pseudoRandomGenerator = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float amplitude = 1;
        float frequency = 1;
        float maxGlobalNoiseValue = 0;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = pseudoRandomGenerator.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = pseudoRandomGenerator.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxGlobalNoiseValue += amplitude;
            amplitude *= settings.persistence;
        }

        float maxLocalNoiseValue = float.MinValue;
        float minLocalNoiseValue = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseValue = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseValue += perlinValue * amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.lacunarity;
                }

                maxLocalNoiseValue = noiseValue > maxLocalNoiseValue ? noiseValue : maxLocalNoiseValue;
                minLocalNoiseValue = noiseValue < minLocalNoiseValue ? noiseValue : minLocalNoiseValue;

                noiseMap[x, y] = noiseValue;

                if (settings.normalizedMode == NormalizedMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / maxGlobalNoiseValue;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        // Normalize height map
        if (settings.normalizedMode == NormalizedMode.Local)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseValue, maxLocalNoiseValue, noiseMap[x, y]);        
                }
            }
        }
        

        return noiseMap;
    }
}


[System.Serializable]
public class NoiseSettings
{
    public NoiseMapGenerator.NormalizedMode normalizedMode;
    public int seed;
    public float scale = 50;
    public int octaves = 15;

    [Range(0, 1)]
    public float persistence = 0.5f;
    public float lacunarity = 2;

    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        persistence = Mathf.Clamp01(persistence);
        lacunarity = Mathf.Max(lacunarity, 1);
    }
}
