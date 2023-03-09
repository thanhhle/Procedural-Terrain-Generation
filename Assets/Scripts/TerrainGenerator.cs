using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float characterMoveThresholdForChunkUpdate = 25f;
    const float sqrCharacterMoveThresholdForChunkUpdate = characterMoveThresholdForChunkUpdate * characterMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODData[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Transform character;
    public Material terrainMaterial;

    Vector2 characterPosition;
    Vector2 lastCharacterPosition;

    private float meshWorldSize;
    private int visibleChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();


    void Start()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        
        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.meshWorldSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UpdateVisibleChunks();
    }


    void Update()
    {
        characterPosition = new Vector2(character.position.x, character.position.z);

        if (characterPosition != lastCharacterPosition)
        {
            foreach(TerrainChunk terrainChunk in visibleTerrainChunks)
            {
                terrainChunk.UpdateCollisionMesh();
            }
        }

        if ((lastCharacterPosition - characterPosition).sqrMagnitude > sqrCharacterMoveThresholdForChunkUpdate)
        {
            lastCharacterPosition = characterPosition;
            UpdateVisibleChunks();
        }
    }


    void UpdateVisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(characterPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(characterPosition.y / meshWorldSize);

        for (int i = 0; i < visibleTerrainChunks.Count; i++)
        {
            visibleTerrainChunks[i].SetVisible(false);
        }

        visibleTerrainChunks.Clear();

        for (int x = -visibleChunks; x <= visibleChunks; x++)
        {
            for (int y = -visibleChunks; y <= visibleChunks; y++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunk terrainChunk = terrainChunkDictionary[viewedChunkCoord];
                    terrainChunk.UpdateTerrainChunk();
                }
                else
                {
                    TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, character, transform, terrainMaterial);
                    terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                    terrainChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                    terrainChunk.Load();
                }
            }
        }
    }


    void OnTerrainChunkVisibilityChanged(TerrainChunk terrainChunk, bool isVisible)
    {
        if(isVisible)
        {
            visibleTerrainChunks.Add(terrainChunk);
        }
        else
        {
            visibleTerrainChunks.Remove(terrainChunk);
        }
    }
}


[System.Serializable]
public struct LODData
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}