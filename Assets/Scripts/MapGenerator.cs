using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool autoUpdate;

    public enum MapType { NoiseMap, FalloffMap, Mesh };
    public MapType mapType;

    public NoiseData noiseData;
    public TerrainData terrainData;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, MeshGenerator.numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;

    [Range(0, MeshGenerator.numSupportedFlatShaddedChunkSizes - 1)]
    public int flatShaddedChunkSizeIndex;

    [Range(0, MeshGenerator.numSupportedLODs - 1)]
    public int editorPreviewLevelOfDetail;

    float[,] falloffMap;

    Queue<ThreadData<MapData>> mapDataThreadDataQueue = new Queue<ThreadData<MapData>>();
    Queue<ThreadData<MeshData>> meshDataThreadDataQueue = new Queue<ThreadData<MeshData>>();


    private void Awake() 
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
    }


    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            RenderMap();
        }
    }


    private void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }


    public int mapChunkSize
    {
        get
        {
            if (terrainData.enableFlatShadding)
            {
                return MeshGenerator.supportedFlatShaddedChunkSizes[flatShaddedChunkSizeIndex] - 1;
            }
            else
            {
                return MeshGenerator.supportedChunkSizes[chunkSizeIndex] - 1;
            }
        }
    }

    public void RenderMap()
    {
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        MapData mapData = GenerateMapData(Vector2.zero);
        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        if (mapType == MapType.NoiseMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromNoiseMap(mapData.noiseMap));
        }
        else if (mapType == MapType.Mesh)
        {
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLevelOfDetail, terrainData.enableFlatShadding));
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail, terrainData.enableFlatShadding);
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


    private MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistence, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizedMode);
        
        if (terrainData.enableFalloffMap)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }
           
            for (int x = 0; x < mapChunkSize + 2; x++)
            {
                for (int y = 0; y < mapChunkSize + 2; y++)
                {
                    if (terrainData.enableFalloffMap)           
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }
                }
            }
        }

        return new MapData(noiseMap);
    }


    private void OnValidate() 
    {    
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }

        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
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


public struct MapData
{
    public readonly float[,] noiseMap;

    public MapData(float[,] noiseMap)
    {
        this.noiseMap = noiseMap;
    }
}
