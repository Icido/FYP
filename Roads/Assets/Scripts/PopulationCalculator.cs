using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationCalculator : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;

    [Range(0f, 1f)]
    public float highDensityLimit = 0.95f;

    [Range(1f, 50f)]
    public float noiseScale;

    public int areaSize;

    private List<Vector3> noiseMap;

    private List<Vector2> tempLocVec = new List<Vector2>();

    private List<Vector2> HighPopDensityLocations = new List<Vector2>();

    [SerializeField]
    private List<Vector2> HighPopDensityAreas = new List<Vector2>();

    //private List<Vector2> MediumPopDensityLocations = new List<Vector2>();

    public void UpdateNoiseMap()
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
        HighPopDensityAreas.Clear();

        for(int i = 0; i < noiseMap.Count; i++)
        {
            if(noiseMap[i].z > highDensityLimit)
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
                if (i == j)
                {
                    continue;
                }

                if((HighPopDensityLocations[i].x + areaSize > HighPopDensityLocations[j].x) && (HighPopDensityLocations[i].x - areaSize < HighPopDensityLocations[j].x) &&
                    (HighPopDensityLocations[i].y + areaSize > HighPopDensityLocations[j].y) && (HighPopDensityLocations[i].y - areaSize < HighPopDensityLocations[j].y))
                {
                    tempLocVec.Add(HighPopDensityLocations[j]);
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

    public List<Vector2> getHighPopAreas()
    {
        return HighPopDensityAreas;
    }
}
