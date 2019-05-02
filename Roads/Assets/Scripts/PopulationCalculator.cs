using System.Collections.Generic;
using UnityEngine;

public static class PopulationCalculator {

    private static List<Vector3> givenHighDensityPopulationLocations;

    private static float populationSizeToTerrainSize;

    private static List<Vector3> tempLocVec = new List<Vector3>();

    private static List<Vector3> HighPopDensityAreas = new List<Vector3>();

    //This generates points of high density population through Perlin Noise, then groups these points if they are close enough given the popAreaSize
    public static void UpdatePopulationMap(float[,] terrainPoints, int populationMapSize, float highDensityLimit, float noiseScale, int populationAreaSize)
    {
        populationSizeToTerrainSize = TerrainCalculator.getTerrainSize();

        populationSizeToTerrainSize /= populationMapSize;

        givenHighDensityPopulationLocations = Noise.GenerateNoiseMapList(populationMapSize, noiseScale, highDensityLimit);
            
        DrawPopulationMap(terrainPoints, populationAreaSize);
        
    }

    public static void DrawPopulationMap(float[,] terrPoints, int populationAreaSize)
    {
        HighPopDensityAreas.Clear();

        for (int i = 0; i < givenHighDensityPopulationLocations.Count; i++)
        {
            tempLocVec.Clear();
            tempLocVec.Add(givenHighDensityPopulationLocations[i]);

            for (int j = 0; j < givenHighDensityPopulationLocations.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if((givenHighDensityPopulationLocations[i].x + populationAreaSize > givenHighDensityPopulationLocations[j].x) && 
                    (givenHighDensityPopulationLocations[i].x - populationAreaSize < givenHighDensityPopulationLocations[j].x) &&
                    (givenHighDensityPopulationLocations[i].z + populationAreaSize > givenHighDensityPopulationLocations[j].z) && 
                    (givenHighDensityPopulationLocations[i].z - populationAreaSize < givenHighDensityPopulationLocations[j].z))
                {
                    tempLocVec.Add(givenHighDensityPopulationLocations[j]);
                }
            }

            int tempCount = tempLocVec.Count;
            float tempX = 0f;
            float tempZ = 0f;

            for (int v = 0; v < tempCount; v++)
            {
                tempX += tempLocVec[v].x;
                tempZ += tempLocVec[v].z;
            }

            float newTempX = tempX / tempCount;
            float newTempZ = tempZ / tempCount;

            Vector3 tempVec3 = new Vector3(Mathf.RoundToInt(newTempX * populationSizeToTerrainSize), 0, Mathf.RoundToInt(newTempZ * populationSizeToTerrainSize));

            if(!HighPopDensityAreas.Contains(tempVec3))
            {
                HighPopDensityAreas.Add(tempVec3);
            }
        }

        for (int i = 0; i < HighPopDensityAreas.Count; i++)
        {
            HighPopDensityAreas[i] = new Vector3(HighPopDensityAreas[i].x, terrPoints[(int)HighPopDensityAreas[i].x, (int)HighPopDensityAreas[i].z], HighPopDensityAreas[i].z);
        }
    }

    public static List<Vector3> getHighPopAreas()
    {
        return HighPopDensityAreas;
    }
}
