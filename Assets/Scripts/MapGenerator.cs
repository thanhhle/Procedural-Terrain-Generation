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
}