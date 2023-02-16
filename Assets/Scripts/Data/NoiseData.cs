using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : AutoUpdateData 
{
    public NoiseMapGenerator.NormalizedMode normalizedMode;
    public int seed;
    public float scale;

    public int octaves;

    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public Vector2 offset;

    protected override void OnValidate()
    {
        scale = scale <= 0 ? 0.1f : scale;
        lacunarity = lacunarity < 1 ? 1 : lacunarity;
        octaves = octaves < 0 ? 0 : octaves;
        
        base.OnValidate();
    }
}
