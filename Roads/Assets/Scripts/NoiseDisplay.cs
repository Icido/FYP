using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDisplay : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;

    [Range(1f, 50f)]
    public float noiseScale;

    private List<Vector3> noiseMap;

    public List<Vector2> HighPopDensityLocations = new List<Vector2>();
    public List<Vector2> MediumPopDensityLocations = new List<Vector2>();

    public void Update()
    {
        List<Vector3> tempNoiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);

        if (tempNoiseMap != noiseMap)
        {
            noiseMap = tempNoiseMap;
            
            DrawNoiseMap(noiseMap);
        }
    }

    public void DrawNoiseMap(List<Vector3> noiseMap)
    {
        HighPopDensityLocations.Clear();

        int size = noiseMap.Count;
        
        for(int i = 0; i < size; i++)
        {
            if(noiseMap[i].z > 0.095f)
            {
                HighPopDensityLocations.Add(new Vector2(noiseMap[i].x, noiseMap[i].y));
            }
        }

    }

    public List<Vector2> getHighPopLocations()
    {
        return HighPopDensityLocations;
    }


}
