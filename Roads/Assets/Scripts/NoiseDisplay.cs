using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDisplay : MonoBehaviour {

    public Renderer textureRender;
    public int mapWidth;
    public int mapHeight;

    [Range(1f, 50f)]
    public float noiseScale;

    private float[,] noiseMap;

    public List<Vector2> PopDensityLocations = new List<Vector2>();

    private Color popLowerBound = new Color(0.95f, 0.95f, 0.95f);

    public void Update()
    {
        float[,] tempNoiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);

        if (tempNoiseMap != noiseMap)
        {
            noiseMap = tempNoiseMap;
            
            DrawNoiseMap(noiseMap);
        }
    }

    public void DrawNoiseMap(float[,] noiseMap)
    {
        PopDensityLocations.Clear();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);

                if(colourMap[y * width + x].r >= popLowerBound.r)
                {
                    PopDensityLocations.Add(new Vector2(x,y));
                }
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, height);

    }

    public List<Vector2> getPopLocations()
    {
        return PopDensityLocations;
    }


}
