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


    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 sampleCenter;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODData[] detailLevels;
        LODMesh[] lodMeshes;
        int colliderLODIndex;

        HeightMap mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;
        bool hasSetCollider;


        public TerrainChunk(Vector2 coord, float meshWorldSize, LODData[] detailLevels, int colliderLODIndex, Transform parent, Material material)
        {
            this.sampleCenter = coord * meshWorldSize / mapGenerator.meshSettings.scale;
            Vector2 position = coord * meshWorldSize;
            this.bounds = new Bounds(position, Vector2.one * meshWorldSize);
            
            this.detailLevels = detailLevels;
            this.colliderLODIndex = colliderLODIndex;

            this.meshObject = new GameObject("Terrain Chunk");
            this.meshRenderer = meshObject.AddComponent<MeshRenderer>();
            this.meshFilter = meshObject.AddComponent<MeshFilter>();
            this.meshCollider = meshObject.AddComponent<MeshCollider>();
            this.meshRenderer.material = material;

            this.meshObject.transform.position = new Vector3(position.x, 0, position.y);
            this.meshObject.transform.parent = parent;

            SetVisible(false);

            this.lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                this.lodMeshes[i] = new LODMesh(detailLevels[i].lod);
                this.lodMeshes[i].updateCallback += UpdateTerrainChunk;

                if (i == colliderLODIndex)
                {
                    this.lodMeshes[i].updateCallback += UpdateCollisionMesh;
                }
            }

            mapGenerator.RequestHeightMap(sampleCenter, OnHeightMapReceived);
        }

        private void OnHeightMapReceived(HeightMap mapData)
        {
            this.mapData = mapData;
            this.mapDataReceived = true;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (this.mapDataReceived)
            {
                float characterDistancefromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(characterPosition));
                bool visible = characterDistancefromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (characterDistancefromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != this.previousLODIndex)
                    {
                        LODMesh lodMesh = this.lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            this.previousLODIndex = lodIndex;
                            this.meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    visibleTerrainChunks.Add(this);     
                }
            
                SetVisible(visible);
            }
        }

        public void UpdateCollisionMesh()
        {
            if (!hasSetCollider)
            {
                float sqrDistanceFromCharacterToEdge = bounds.SqrDistance(characterPosition);

                if (sqrDistanceFromCharacterToEdge < detailLevels[colliderLODIndex].sqrVisibleDistanceThreshold)
                {
                    if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                    {
                        lodMeshes[colliderLODIndex].RequestMesh(mapData);
                    }
                }

                if (sqrDistanceFromCharacterToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
                {
                    if (lodMeshes[colliderLODIndex].hasMesh)
                    {
                        meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                        hasSetCollider = true;
                    }
                }
            }    
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }


    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;
        public event System.Action updateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            this.hasMesh = true;
            this.updateCallback();
        }

        public void RequestMesh(HeightMap mapData)
        {
            this.hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, this.lod, OnMeshDataReceived);

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
}