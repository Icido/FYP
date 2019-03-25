using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationCalculator : MonoBehaviour {

    public int mapSize;

    [Range(0f, 1f)]
    public float highDensityLimit = 0.95f;

    [Range(1f, 50f)]
    public float noiseScale;

    public int populationAreaSize;

    public int seed;

    private List<Vector3> noiseMap;

    private List<Vector2> tempLocVec = new List<Vector2>();

    private List<Vector2> HighPopDensityLocations = new List<Vector2>();

    private List<Vector3> HighPopDensityAreas = new List<Vector3>();

    public void UpdatePopulationMap(float[,] terrainPoints)
    {
        List<Vector3> tempPopMap = Noise.GenerateNoiseMapList(mapSize, noiseScale);

        if (tempPopMap != noiseMap)
        {
            noiseMap = tempPopMap;
            
            DrawPopulationMap(noiseMap, terrainPoints);
        }
    }

    public void DrawPopulationMap(List<Vector3> noiseMap, float[,] terrainPoints)
    {
        HighPopDensityLocations.Clear();
        HighPopDensityAreas.Clear();

        for(int i = 0; i < noiseMap.Count; i++)
        {
            if(noiseMap[i].y > highDensityLimit)
            {
                HighPopDensityLocations.Add(new Vector2(noiseMap[i].x, noiseMap[i].z));
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

                if((HighPopDensityLocations[i].x + populationAreaSize > HighPopDensityLocations[j].x) && (HighPopDensityLocations[i].x - populationAreaSize < HighPopDensityLocations[j].x) &&
                    (HighPopDensityLocations[i].y + populationAreaSize > HighPopDensityLocations[j].y) && (HighPopDensityLocations[i].y - populationAreaSize < HighPopDensityLocations[j].y))
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

            Vector3 tempVec3 = new Vector3((int)(tempX / tempCount), 0, (int)(tempY / tempCount));

            if(!HighPopDensityAreas.Contains(tempVec3))
            {
                HighPopDensityAreas.Add(tempVec3);
            }
        }

        for (int i = 0; i < HighPopDensityAreas.Count; i++)
        {
            int x = (int)HighPopDensityAreas[i].x;
            int y = (int)HighPopDensityAreas[i].y;

            HighPopDensityAreas[i] = new Vector3(HighPopDensityAreas[i].x, terrainPoints[x, y], HighPopDensityAreas[i].z);

        }
    }

    public List<Vector3> getHighPopAreas()
    {
        return HighPopDensityAreas;
    }

    public int getMapSize()
    {
        return mapSize;
    }
}
