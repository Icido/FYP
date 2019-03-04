using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDisplay : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;

    [Range(1f, 50f)]
    public float noiseScale;

    private List<Vector3> noiseMap;

    private List<Vector2> tempLocVec = new List<Vector2>();

    public List<Vector2> HighPopDensityLocations = new List<Vector2>();
    public List<Vector2> HighPopDensityAreas = new List<Vector2>();
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

        for(int i = 0; i < noiseMap.Count; i++)
        {
            if(noiseMap[i].z > 0.95f)
            {
                HighPopDensityLocations.Add(new Vector2(noiseMap[i].x, noiseMap[i].y));
            }
        }

        for (int i = 0; i < HighPopDensityLocations.Count; i++)
        {
            tempLocVec.Clear();
            tempLocVec.Add(HighPopDensityLocations[i]);

            for (int j = 0; j < HighPopDensityLocations.Count; j++)
            {
                if(i != j)
                {
                    if(HighPopDensityLocations[i].x + 6 > HighPopDensityLocations[j].x &&
                       HighPopDensityLocations[i].x - 6 < HighPopDensityLocations[j].x)
                    {
                        if (HighPopDensityLocations[i].y + 6 > HighPopDensityLocations[j].y &&
                            HighPopDensityLocations[i].y - 6 < HighPopDensityLocations[j].y)
                        {
                            tempLocVec.Add(HighPopDensityLocations[i]);
                        }
                    }
                }
            }

            int tempCount = tempLocVec.Count;
            float tempX = 0f;
            float tempY = 0f;

            for (int v = 0; v < tempCount; v++)
            {
                tempX += tempLocVec[v].x;
                tempY += tempLocVec[v].y;
            }

            Vector2 tempVec2 = new Vector2(tempX / tempCount, tempY / tempCount);

            if(!HighPopDensityAreas.Contains(tempVec2))
            {
                HighPopDensityAreas.Add(tempVec2);
            }



        }


    }

    public List<Vector2> getHighPopLocations()
    {
        return HighPopDensityAreas;
    }


}
