using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public const int mapChunkSize = 241;

    public bool autoUpdate;

    public enum MapType { NoiseMap, FalloffMap, ColorMap, Mesh };
    public MapType mapType;

    public NoiseMapGenerator.NormalizedMode normalizedMode;

    public bool enableFalloffMap;

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
    public int editorPreviewLevelOfDetail;

    public Landform[] landforms;

    float[,] falloffMap;

    Queue<ThreadData<MapData>> mapDataThreadDataQueue = new Queue<ThreadData<MapData>>();
    Queue<ThreadData<MeshData>> meshDataThreadDataQueue = new Queue<ThreadData<MeshData>>();

    private void Awake() 
    {
        falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapChunkSize);
    }

    public void RenderMap()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
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
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLevelOfDetail), TextureGenerator.GenerateTextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (mapType == MapType.FalloffMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromNoiseMap(FalloffMapGenerator.GenerateFalloffMap(mapChunkSize)));
        }
    }


    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }


    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadDataQueue)
        {
            mapDataThreadDataQueue.Enqueue(new ThreadData<MapData>(callback, mapData));
        }
    }


    public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, levelOfDetail, callback);
        };

        new Thread(threadStart).Start();
    }


    private void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
        lock (meshDataThreadDataQueue)
        {
            meshDataThreadDataQueue.Enqueue(new ThreadData<MeshData>(callback, meshData));
        }
    }


    private void Update()
    {
        if (mapDataThreadDataQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadDataQueue.Count; i++)
            {
                ThreadData<MapData> threadInfo = mapDataThreadDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadDataQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadDataQueue.Count; i++)
            {
                ThreadData<MeshData> threadInfo = meshDataThreadDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistence, lacunarity, center + offset, normalizedMode);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                if (enableFalloffMap)           
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                float noiseValue = noiseMap[x, y];
                for (int i = 0; i < landforms.Length; i++)
                {
                    if (noiseValue >= landforms[i].threshold)
                    {
                        colorMap[y * mapChunkSize + x] = landforms[i].color;
                    }
                    else
                    {
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

        falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapChunkSize);
    }


    struct ThreadData<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public ThreadData(Action<T> callback, T parameter)
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
