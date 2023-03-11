using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public bool autoUpdate;

    public enum MapType { HeightMap, FalloffMap, Mesh };
    public MapType mapType;
    
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int editorPreviewLevelOfDetail;

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public TextureSettings textureSettings;
    public Material terrainMaterial;

    public void RenderMap()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVerticesPerLine, meshSettings.numVerticesPerLine, heightMapSettings, Vector2.zero);
        if (mapType == MapType.HeightMap)
        {
            RenderTexture(TextureGenerator.GenerateTextureFromHeightMap(heightMap));
        }
        else if (mapType == MapType.Mesh)
        {
            RenderMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLevelOfDetail));
        }
        else if (mapType == MapType.FalloffMap)
        {
            RenderTexture(TextureGenerator.GenerateTextureFromHeightMap(new HeightMap(FalloffMapGenerator.GenerateFalloffMap(meshSettings.numVerticesPerLine), 0, 1)));
        }
    }


    public void RenderTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }


    public void RenderMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
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
        textureSettings.ApplyToMaterial(terrainMaterial);
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

        if (textureSettings != null)
        {
            textureSettings.OnValuesUpdated -= OnTextureValuesUpdated;
            textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
