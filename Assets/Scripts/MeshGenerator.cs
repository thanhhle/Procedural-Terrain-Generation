using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int simplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;

        int borderSize = noiseMap.GetLength(0);
        int meshSize = borderSize - 2 * simplificationIncrement;
        int meshSizeUnsimplified = borderSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / 2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;
   
        int verticesPerLine = (meshSize - 1) / simplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertexIndexMap = new int[borderSize, borderSize];
        int borderVertexIndex = -1;
        int meshVertexIndex = 0;

        for (int x = 0; x < borderSize; x += simplificationIncrement)
        {
            for (int y = 0; y < borderSize; y += simplificationIncrement)
            {
                bool isBorderVertex = x == 0 || x == borderSize - 1 || y == 0 || y == borderSize - 1;
                if (isBorderVertex)
                {
                    vertexIndexMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndexMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }


        for (int x = 0; x < borderSize; x += simplificationIncrement)
        {
            for (int y = 0; y < borderSize; y += simplificationIncrement)
            {
                int vertexIndex = vertexIndexMap[x, y];
                Vector2 uv = new Vector2((x - simplificationIncrement) / (float)meshSize, (y - simplificationIncrement) / (float)meshSize);
                float heightValue = heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier;
                Vector3 vertexPosition = new Vector3 (topLeftX - uv.x * meshSizeUnsimplified, heightValue, uv.y * meshSizeUnsimplified - topLeftZ);

                meshData.AddVertex(vertexPosition, uv, vertexIndex);

                if (x < borderSize - 1 && y < borderSize - 1)
                {
                    int a = vertexIndexMap[x, y];
                    int b = vertexIndexMap[x + simplificationIncrement, y];
                    int c = vertexIndexMap[x, y + simplificationIncrement];
                    int d = vertexIndexMap[x + simplificationIncrement, y + simplificationIncrement];

                    meshData.AddTriangle(d, a, b);
                    meshData.AddTriangle(a, d, c);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}


public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private Vector3[] borderVertices;
    private int[] borderTriangles;

    private int triangleIndex;
    private int borderTriangleIndex;

    public MeshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
        uvs = new Vector2[verticesPerLine * verticesPerLine];

        borderVertices = new Vector3[verticesPerLine * 4 + 4];
        borderTriangles = new int[verticesPerLine * 24];
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            borderVertices[-vertexIndex - 1] = vertexPosition;
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
            borderTriangles[borderTriangleIndex] = vertexA;
            borderTriangles[borderTriangleIndex + 1] = vertexB;
            borderTriangles[borderTriangleIndex + 2] = vertexC;
            borderTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = vertexA;
            triangles[triangleIndex + 1] = vertexB;
            triangles[triangleIndex + 2] = vertexC;
            triangleIndex += 3;
        }
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

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC= borderTriangles[normalTriangleIndex + 2];

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

    private Vector3 CalculateSurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = indexA < 0 ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = indexB < 0 ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = indexC < 0 ? borderVertices[-indexC - 1] : vertices[indexC];
        
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }


    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = CalculateNormals();
        return mesh;
    }
}


