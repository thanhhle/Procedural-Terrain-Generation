using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public Renderer TextureRenderer;

    public void RenderMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D textureMap = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                textureMap.SetPixel(x, y, color);
            }
        }
        
        textureMap.Apply();

        TextureRenderer.sharedMaterial.mainTexture = textureMap;
        TextureRenderer.transform.localScale = new Vector3(width, 1, height);
    }
}
