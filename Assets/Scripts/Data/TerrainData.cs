using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : AutoUpdateData 
{
    public float uniformScale = 2.5f;
    public bool enableFlatShadding;
    public bool enableFalloffMap;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
}
