using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMapGenerator
{
    public enum NormalizedMode {Local, Global}

    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, NormalizedMode normalizedMode)
    {
        float[,] noiseMap = new float[width, height];

        System.Random pseudoRandomGenerator = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        float frequency = 1;
        float maxGlobalNoiseValue = 0;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = pseudoRandomGenerator.Next(-100000, 100000) + offset.x;
            float offsetY = pseudoRandomGenerator.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxGlobalNoiseValue += amplitude;
            amplitude *= persistence;
        }

        scale = scale <= 0 ? 0.0001f : scale;

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

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth - octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight - octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                maxLocalNoiseValue = noiseValue > maxLocalNoiseValue ? noiseValue : maxLocalNoiseValue;
                minLocalNoiseValue = noiseValue < minLocalNoiseValue ? noiseValue : minLocalNoiseValue;

                noiseMap[x, y] = noiseValue;
            }
        }

        // Normalize noise map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (normalizedMode == NormalizedMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseValue, maxLocalNoiseValue, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / maxGlobalNoiseValue;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
