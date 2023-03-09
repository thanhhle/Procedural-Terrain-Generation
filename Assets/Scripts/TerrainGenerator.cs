using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float characterMoveThresholdForChunkUpdate = 25f;
    const float sqrCharacterMoveThresholdForChunkUpdate = characterMoveThresholdForChunkUpdate * characterMoveThresholdForChunkUpdate;
    const float colliderGenerationDistanceThreshold = 5;

    public int colliderLODIndex;
    public LODData[] detailLevels;
    public static float maxViewDistance;

    public Transform character;
    public Material mapMaterial;

    public static Vector2 characterPosition;
    Vector2 lastCharacterPosition;

    static MapGenerator mapGenerator;
    private float meshWorldSize;
    private int visibleChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();


    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshWorldSize = mapGenerator.meshSettings.meshWorldSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UpdateVisibleChunks();
    }


    void Update()
    {
        characterPosition = new Vector2(character.position.x, character.position.z);

        if (characterPosition != lastCharacterPosition)
        {
            foreach(TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
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
                    TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, meshWorldSize, detailLevels, colliderLODIndex, transform, mapMaterial);
                    terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                }
            }
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