using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : AutoUpdate
{
    public float scale = 2.5f;
    public bool enableFlatShadding;

    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatShaddedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = {48, 72, 96, 120, 144, 168, 192, 216, 240};

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;

    [Range(0, numSupportedFlatShaddedChunkSizes - 1)]
    public int flatShaddedChunkSizeIndex;

    // number of vertices per line of mesh rendered at LOD = 0
    // includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals
    public int numVerticesPerLine
    {
        get
        {
            return supportedChunkSizes[(enableFlatShadding) ? flatShaddedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }


    public float meshWorldSize
    {
        get
        {
            return (numVerticesPerLine - 3) * scale;
        }
    }
}
