using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
    {
        int skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int numVerticesPerLine = meshSettings.numVerticesPerLine;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        MeshData meshData = new MeshData(numVerticesPerLine, skipIncrement, meshSettings.enableFlatShadding);

        int[,] vertexIndexMap = new int[numVerticesPerLine, numVerticesPerLine];
        int outOfMeshVertexIndex = -1;
        int meshVertexIndex = 0;

        for (int x = 0; x < numVerticesPerLine; x++)   
        {
            for (int y = 0; y < numVerticesPerLine; y++)        
            {
                bool isOutOfMeshVertex = x == 0 || x == numVerticesPerLine - 1 || y == 0 || y == numVerticesPerLine - 1;
                bool isSkippedVertex = x > 2 && x < numVerticesPerLine - 3 && y > 2 && y < numVerticesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutOfMeshVertex)
                {
                    vertexIndexMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if (!isSkippedVertex)
                {
                    vertexIndexMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int x = 0; x < numVerticesPerLine; x++)
        {
            for (int y = 0; y < numVerticesPerLine; y++)    
            {
                bool isSkippedVertex = x > 2 && x < numVerticesPerLine - 3 && y > 2 && y < numVerticesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex)
                {
                    bool isOutOfMeshVertex = x == 0 || x == numVerticesPerLine - 1 || y == 0 || y == numVerticesPerLine - 1;
                    bool isMeshEdgeVertex = (x == 1 || x == numVerticesPerLine - 2 || y == 1 || y == numVerticesPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (x == 2 || x == numVerticesPerLine - 3 || y == 2 || y == numVerticesPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndexMap[x, y];
                    float heightValue = heightMap[x, y];
                    
                    Vector2 uv = new Vector2(x - 1, y - 1) / (numVerticesPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(uv.x, -uv.y) * meshSettings.meshWorldSize;

                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVerticesPerLine - 3;

                        int distanceToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
                        int distanceToMainVertexB = skipIncrement - distanceToMainVertexA;

                        float distancePercentFromAToB = distanceToMainVertexA / (float)skipIncrement;

                        Coord coordA = new Coord((isVertical) ? x : x - distanceToMainVertexA, (isVertical) ? y - distanceToMainVertexA : y);
                        Coord coordB = new Coord((isVertical) ? x : x + distanceToMainVertexB, (isVertical) ? y + distanceToMainVertexB : y);

                        float heightMainVertexA = heightMap[coordA.x, coordA.y];
                        float heightMainVertexB = heightMap[coordB.x, coordB.y];

                        heightValue = heightMainVertexA * (1 - distancePercentFromAToB) + heightMainVertexB * distancePercentFromAToB;

                        EdgeConnectionVertexData edgeConnectionVertexData = new EdgeConnectionVertexData(vertexIndex, vertexIndexMap[coordA.x, coordA.y], vertexIndexMap[coordB.x, coordB.y], distancePercentFromAToB);
                        meshData.DeclareEdgeConnectionVertex(edgeConnectionVertexData);
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, heightValue, vertexPosition2D.y), uv, vertexIndex);

                    bool createTriangle = x < numVerticesPerLine - 1 && y < numVerticesPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));
                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != numVerticesPerLine - 3 && y != numVerticesPerLine - 3) ? skipIncrement : 1;
                        int vertexA = vertexIndexMap[x, y];
                        int vertexB = vertexIndexMap[x + currentIncrement, y];
                        int vertexC = vertexIndexMap[x, y + currentIncrement];
                        int vertexD = vertexIndexMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(vertexA, vertexD, vertexC);
                        meshData.AddTriangle(vertexD, vertexA, vertexB);
                    }
                }
            }
        }

        meshData.ProcessMesh();

        return meshData;
    }


    public struct Coord
    {
        public readonly int x;
        public readonly int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}


public class EdgeConnectionVertexData
{
    public int vertexIndex;
    public int mainVertexAIndex;
    public int mainVertexBIndex;
    public float distancePercentFromAToB;

    public EdgeConnectionVertexData(int vertexIndex, int mainVertexAIndex, int mainVertexBIndex, float distancePercentFromAToB)
    {
        this.vertexIndex = vertexIndex;
        this.mainVertexAIndex = mainVertexAIndex;
        this.mainVertexBIndex = mainVertexBIndex;
        this.distancePercentFromAToB = distancePercentFromAToB;
    }
}


public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Vector3[] bakedNormals;

    private Vector3[] outOfMeshVertices;
    private int[] outOfMeshTriangles;

    private int triangleIndex;
    private int outOfMeshTriangleIndex;

    EdgeConnectionVertexData[] edgeConnectionVertices;
    int edgeConnectionVertexIndex;

    private bool enableFlatShadding;

    public MeshData(int numVerticesPerLine, int skipIncrement, bool enableFlatShadding)
    {
        int numMeshEdgeVertices = (numVerticesPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVerticesPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVerticesPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
        uvs = new Vector2[vertices.Length];
        edgeConnectionVertices = new EdgeConnectionVertexData[numEdgeConnectionVertices];

        int numMeshEdgeTriangles = (numVerticesPerLine - 4) * 8;
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

        outOfMeshVertices = new Vector3[numVerticesPerLine * 4 - 4];
        outOfMeshTriangles = new int[(numVerticesPerLine - 2) * 24];

        this.enableFlatShadding = enableFlatShadding;
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int vertexA, int vertexB, int vertexC)
    {
        if (vertexA < 0 || vertexB < 0 || vertexC < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = vertexA;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = vertexB;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = vertexC;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = vertexA;
            triangles[triangleIndex + 1] = vertexB;
            triangles[triangleIndex + 2] = vertexC;
            triangleIndex += 3;
        }
    }

    public void DeclareEdgeConnectionVertex(EdgeConnectionVertexData edgeConnectionVertexData)
    {
        edgeConnectionVertices[edgeConnectionVertexIndex] = edgeConnectionVertexData;
        edgeConnectionVertexIndex++;
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];

        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC= triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = CalculateSurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC= outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = CalculateSurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

            if (vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }

            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }

            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }


    private void ProcessEdgeConnectionVertices()
    {
        foreach(EdgeConnectionVertexData e in edgeConnectionVertices)
        {
            bakedNormals[e.vertexIndex] = bakedNormals[e.mainVertexAIndex] * (1 - e.distancePercentFromAToB) + bakedNormals[e.mainVertexBIndex] * e.distancePercentFromAToB;
        }
    }


    private Vector3 CalculateSurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = indexA < 0 ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = indexB < 0 ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = indexC < 0 ? outOfMeshVertices[-indexC - 1] : vertices[indexC];
        
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }


    public void ProcessMesh()
    {
        if (enableFlatShadding)
        {
            FlatShading();
        }
        else
        {
            BakeNormals();
            ProcessEdgeConnectionVertices();
        }
    }


    private void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }


    public void FlatShading()
    {
        Vector3[] flatShaddedVertices = new Vector3[triangles.Length];
        Vector2[] flatShaddedUVs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShaddedVertices[i] = vertices[triangles[i]];
            flatShaddedUVs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShaddedVertices;
        uvs = flatShaddedUVs;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        if (enableFlatShadding)
        {
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.normals = bakedNormals;
        }
 
        return mesh;
    }
}


