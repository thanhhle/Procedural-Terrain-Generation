using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float noiseScale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        noiseScale = noiseScale <= 0 ? 0.0001f : noiseScale;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float sampleX = x / noiseScale;
                float sampleY = y / noiseScale;

                noiseMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }

        return noiseMap;
    }
}
