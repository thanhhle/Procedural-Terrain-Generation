using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D textureMap = new Texture2D(mapWidth, mapHeight);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        textureMap.SetPixels(colorMap);
        textureMap.Apply();

        TextureRenderer.sharedMaterial.mainTexture = textureMap;
        TextureRenderer.transform.localScale = new Vector3(mapWidth, 1, mapHeight);
    }
}
