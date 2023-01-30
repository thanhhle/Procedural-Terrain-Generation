using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public enum MapType { NoiseMap, ColorMap, Mesh };
    public MapType mapType;

    public int seed;
    public float scale;

    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    [Range(0, 6)]
    public int levelOfDetail;

    public Landform[] landforms;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void RenderMap()
    {
        MapData mapData = GenerateMapData();
        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        if (mapType == MapType.NoiseMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromNoiseMap(mapData.noiseMap));
        }
        else if (mapType == MapType.ColorMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (mapType == MapType.Mesh)
        {
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.GenerateTextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }


    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }


    private void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }


    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }


    private void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }


    private void Update() 
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistence, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                float noiseValue = noiseMap[x, y];
                for (int i = 0; i < landforms.Length; i++)
                {
                    if (noiseValue <= landforms[i].threshold)
                    {
                        colorMap[y * mapChunkSize + x] = landforms[i].color;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }


    private void OnValidate()
    {
        scale = scale <= 0 ? 0.1f : scale;
        lacunarity = lacunarity < 1 ? 1 : lacunarity;
        octaves = octaves < 0 ? 0 : octaves;
    }


    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}


[System.Serializable]
public struct Landform
{
    public string name;
    public float threshold;
    public Color color;
}


public struct MapData
{
    public readonly float[,] noiseMap;
    public readonly Color[] colorMap;

    public MapData(float[,] noiseMap, Color[] colorMap)
    {
        this.noiseMap = noiseMap;
        this.colorMap = colorMap;
    }
}
