using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    const float colliderGenerationDistanceThreshold = 5;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODData[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDistance;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform character;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODData[] detailLevels, int colliderLODIndex, Transform parent, Transform character, Material material)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.character = character;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;

        this.sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.scale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        this.bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

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

        this.maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
    }


    public void Load()
    {
        ThreadDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVerticesPerLine, meshSettings.numVerticesPerLine, heightMapSettings, sampleCenter), OnHeightMapReceived);
    }


    private void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        this.heightMapReceived = true;

        UpdateTerrainChunk();
    }


    Vector2 characterPosition
    {
        get
        {
            return new Vector2(character.position.x, character.position.z);
        }
    }


    public void UpdateTerrainChunk()
    {
        if (this.heightMapReceived)
        {
            float characterDistancefromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(characterPosition));

            bool wasVisible = IsVisible();
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
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }
                }
            }


            if (wasVisible != visible) 
            {	
				SetVisible(visible);
				if (onVisibilityChanged != null) 
                {
					onVisibilityChanged(this, visible);
				}
			}
        }
    }


    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDistanceFromCharacterToEdge = bounds.SqrDistance(characterPosition);
            float characterDistancefromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(characterPosition));

            if (sqrDistanceFromCharacterToEdge < detailLevels[colliderLODIndex].sqrVisibleDistanceThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (characterDistancefromNearestEdge < colliderGenerationDistanceThreshold)
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

    private void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        this.hasMesh = true;
        this.updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        this.hasRequestedMesh = true;
        ThreadDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }
}

