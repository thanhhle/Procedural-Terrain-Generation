using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool autoUpdate;

    public enum MapType { NoiseMap, ColorMap, Mesh };
    public MapType mapType;

    public int width;
    public int height;

    public int seed;
    public float scale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public Vector2 offset;

    public Landform[] landforms;


    public void GenerateMap()
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = noiseMap[x, y];
                for (int i = 0; i < landforms.Length; i++)
                {
                    if (noiseValue <= landforms[i].threshold)
                    {
                        colorMap[y * width + x] = landforms[i].color;
                        break;
                    }
                }
            }
        }

        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        if (mapType == MapType.NoiseMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromNoiseMap(noiseMap));
        }
        else if (mapType == MapType.ColorMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromColorMap(colorMap, width, height));
        }
        else if (mapType == MapType.Mesh)
        {
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.GenerateTextureFromColorMap(colorMap, width, height));
        }
    }


    private void OnValidate()
    {
        width = width < 1 ? 1 : width;
        height = height < 1 ? 1 : height;
        scale = scale <= 0 ? 0.1f : scale;
        lacunarity = lacunarity < 1 ? 1 : lacunarity;
        octaves = octaves < 0 ? 0 : octaves;
    }

}


[System.Serializable]
public struct Landform
{
    public string name;
    public float threshold;
    public Color color;
}
