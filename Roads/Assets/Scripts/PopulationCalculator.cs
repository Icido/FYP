using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PopulationCalculator {

    private static List<Vector3> noiseMap;

    private static float populationSizeToTerrainSize;

    private static List<Vector2> tempLocVec = new List<Vector2>();

    private static List<Vector2> HighPopDensityLocations = new List<Vector2>();

    private static List<Vector3> HighPopDensityAreas = new List<Vector3>();

    public static void UpdatePopulationMap(float[,] terrainPoints, int populationMapSize, float highDensityLimit, float noiseScale, int populationAreaSize)
    {
        List<Vector3> tempPopMap = Noise.GenerateNoiseMapList(populationMapSize, noiseScale);

        populationSizeToTerrainSize = terrainPoints.GetUpperBound(0) + 1;

        populationSizeToTerrainSize /= populationMapSize;

        noiseMap = tempPopMap;
            
        DrawPopulationMap(noiseMap, terrainPoints, highDensityLimit, populationAreaSize);
        
    }

    public static void DrawPopulationMap(List<Vector3> noiseMap, float[,] terrainPoints, float highDensityLimit, int populationAreaSize)
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

            float newTempX = tempX / tempCount;
            float newTempY = tempY / tempCount;

            Vector3 tempVec3 = new Vector3(Mathf.RoundToInt(newTempX * populationSizeToTerrainSize), 0, Mathf.RoundToInt(newTempY * populationSizeToTerrainSize));

            if(!HighPopDensityAreas.Contains(tempVec3))
            {
                HighPopDensityAreas.Add(tempVec3);
            }
        }

        for (int i = 0; i < HighPopDensityAreas.Count; i++)
        {
            HighPopDensityAreas[i] = new Vector3(HighPopDensityAreas[i].x, terrainPoints[(int)HighPopDensityAreas[i].x, (int)HighPopDensityAreas[i].z], HighPopDensityAreas[i].z);
        }
    }

    public static List<Vector3> getHighPopAreas()
    {
        return HighPopDensityAreas;
    }
}
