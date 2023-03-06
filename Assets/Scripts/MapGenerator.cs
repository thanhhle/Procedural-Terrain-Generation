using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool autoUpdate;

    public enum MapType { HeightMap, FalloffMap, Mesh };
    public MapType mapType;

    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLevelOfDetail;

    float[,] falloffMap;

    Queue<ThreadData<HeightMap>> heightMapThreadDataQueue = new Queue<ThreadData<HeightMap>>();
    Queue<ThreadData<MeshData>> meshDataThreadDataQueue = new Queue<ThreadData<MeshData>>();


    void Start() 
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
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

    public void RenderMap()
    {
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVerticesPerLine, meshSettings.numVerticesPerLine, heightMapSettings, Vector2.zero);
        MapRenderer mapRenderer = FindObjectOfType<MapRenderer>();
        if (mapType == MapType.HeightMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromHeightMap(heightMap.values));
        }
        else if (mapType == MapType.Mesh)
        {
            mapRenderer.RenderMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, editorPreviewLevelOfDetail, meshSettings));
        }
        else if (mapType == MapType.FalloffMap)
        {
            mapRenderer.RenderMap(TextureGenerator.GenerateTextureFromHeightMap(FalloffMapGenerator.GenerateFalloffMap(meshSettings.numVerticesPerLine)));
        }
    }


    public void RequestHeightMap(Vector2 center, Action<HeightMap> callback)
    {
        ThreadStart threadStart = delegate
        {
            HeightMapThread(center, callback);
        };

        new Thread(threadStart).Start();
    }


    private void HeightMapThread(Vector2 center, Action<HeightMap> callback)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVerticesPerLine, meshSettings.numVerticesPerLine, heightMapSettings, center);
        lock (heightMapThreadDataQueue)
        {
            heightMapThreadDataQueue.Enqueue(new ThreadData<HeightMap>(callback, heightMap));
        }
    }


    public void RequestMeshData(HeightMap heightMap, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(heightMap, levelOfDetail, callback);
        };

        new Thread(threadStart).Start();
    }


    private void MeshDataThread(HeightMap heightMap, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, levelOfDetail, meshSettings);
        lock (meshDataThreadDataQueue)
        {
            meshDataThreadDataQueue.Enqueue(new ThreadData<MeshData>(callback, meshData));
        }
    }


    private void Update()
    {
        if (heightMapThreadDataQueue.Count > 0)
        {
            for (int i = 0; i < heightMapThreadDataQueue.Count; i++)
            {
                ThreadData<HeightMap> threadInfo = heightMapThreadDataQueue.Dequeue();
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


    private void OnValidate() 
    {    
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
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