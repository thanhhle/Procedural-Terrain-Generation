using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public LevelOfDetailData[] detailLevels;
    public static float maxViewDistance;

    public Transform character;
    public Material mapMaterial;

    public static Vector2 characterPosition;
    static MapGenerator mapGenerator;
    private int chunkSize;
    private int visibleChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleChunksLastUpdate = new List<TerrainChunk>();


    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        visibleChunks = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }


    void Update()
    {
        characterPosition = new Vector2(character.position.x, character.position.z);
        UpdateVisibleChunks();
    }


    void OnDestroy()
    {
        visibleChunksLastUpdate.Clear();
    }


    void UpdateVisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(characterPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(characterPosition.y / chunkSize);

        for (int i = 0; i < visibleChunksLastUpdate.Count; i++)
        {
            visibleChunksLastUpdate[i].SetVisible(false);
        }

        visibleChunksLastUpdate.Clear();

        for (int x = -visibleChunks; x <= visibleChunks; x++)
        {
            for (int y = -visibleChunks; y <= visibleChunks; y++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunk terrainChunk = terrainChunkDictionary[viewedChunkCoord];
                    terrainChunk.Update();
                    if (terrainChunk.IsVisible())
                    {
                        visibleChunksLastUpdate.Add(terrainChunk);
                    }
                }
                else
                {
                    TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial);
                    terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                }
            }
        }
    }


    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LevelOfDetailData[] detailLevels;
        LevelOfDetailMesh[] levelOfDetailMeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLevelOfDetailIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LevelOfDetailData[] detailLevels, Transform parent, Material material)
        {
            this.position = coord * size;
            this.bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            this.detailLevels = detailLevels;

            this.meshObject = new GameObject("Terrain Chunk");
            this.meshRenderer = meshObject.AddComponent<MeshRenderer>();
            this.meshFilter = meshObject.AddComponent<MeshFilter>();
            this.meshRenderer.material = material;

            this.meshObject.transform.position = positionV3;
            this.meshObject.transform.parent = parent;

            SetVisible(false);

            this.levelOfDetailMeshes = new LevelOfDetailMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                this.levelOfDetailMeshes[i] = new LevelOfDetailMesh(detailLevels[i].levelOfDetail);
            }

            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            this.mapDataReceived = true;
        }

        public void Update()
        {
            if (this.mapDataReceived)
            {
                float characterDistancefromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(characterPosition));
                bool visible = characterDistancefromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    int levelOfDetailIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (characterDistancefromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                        {
                            levelOfDetailIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (levelOfDetailIndex != this.previousLevelOfDetailIndex)
                    {
                        LevelOfDetailMesh levelOfDetailMesh = this.levelOfDetailMeshes[levelOfDetailIndex];
                        if (levelOfDetailMesh.hasMesh)
                        {
                            this.previousLevelOfDetailIndex = levelOfDetailIndex;
                            this.meshFilter.mesh = levelOfDetailMesh.mesh;
                        }
                        else if (!levelOfDetailMesh.hasRequestedMesh)
                        {
                            levelOfDetailMesh.RequestMesh(mapData);
                        }
                    }
                }
            }

            SetVisible(visible);
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


    class LevelOfDetailMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int levelOfDetail;

        public LevelOfDetailMesh(int levelOfDetail)
        {
            this.levelOfDetail = levelOfDetail;
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            this.hasMesh = true;
        }

        public void RequestMesh(MapData mapData)
        {
            this.hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, this.levelOfDetail, OnMeshDataReceived);

        }
    }


    [System.Serializable]
    public struct LevelOfDetailData
    {
        public int levelOfDetail;
        public float visibleDistanceThreshold;


    }
}