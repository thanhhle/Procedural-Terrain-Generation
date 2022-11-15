using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Width;
    public int Height;

    public int Seed;
    public float Scale;

    public int Octaves;  
    [Range(0, 1)]
    public float Persistence;
    public float Lacunarity;

    public Vector2 Offset;

    public bool AutoUpdate;


    public void GenerateMap()
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(Width, Height, Seed, Scale, Octaves, Persistence, Lacunarity, Offset);

        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        mapRenderer.RenderMap(noiseMap);
    }


    private void Start() 
    {
        GenerateMap();
    }


    private void OnValidate() 
    {
        Width = Width < 1 ? 1 : Width;
        Height = Height < 1 ? 1 : Height;
        Lacunarity = Lacunarity < 1 ? 1 : Lacunarity;
        Octaves = Octaves < 0 ? 0 : Octaves;
    }
   
}


public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
