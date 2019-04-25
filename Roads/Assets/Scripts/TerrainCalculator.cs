using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainCalculator {

    private static float[,] terrainMap;

    //private float maxTerrainValue = float.MinValue;

    //private float minTerrainValue = float.MaxValue;

    //private float averageTerrainValue = 0f;

    public static void UpdateTerrainMap(int mapSize, float noiseScale, float amplitude)
    {
        float[,] tempTerMap = Noise.GenerateNoiseMapArray(mapSize, noiseScale, amplitude);


        if (tempTerMap != terrainMap)
        {
            terrainMap = tempTerMap;

            //GatherTerrainData(terrainMap);
        }
    }

    public static float[,] getTerrainPoints()
    {
        return terrainMap;
    }

    //public void GatherTerrainData(float[,] noiseMap)
    //{
    //    int mapSize = noiseMap.GetUpperBound(0) + 1;

    //    for (int y = 0; y < mapSize; y++)
    //    {
    //        for (int x = 0; x < mapSize; x++)
    //        {
    //            if (maxTerrainValue < noiseMap[x, y])
    //                maxTerrainValue = noiseMap[x, y];

    //            if (minTerrainValue > noiseMap[x, y])
    //                minTerrainValue = noiseMap[x, y];

    //            averageTerrainValue += noiseMap[x, y];
    //        }
    //    }

    //    averageTerrainValue /= (mapSize * mapSize);

    //    return;
    //}

    //public float getMaxTerrainValue()
    //{
    //    return maxTerrainValue;
    //}

    //public float getMinTerrainValue()
    //{
    //    return minTerrainValue;
    //}

    //public float getAverageTerrainValue()
    //{
    //    return averageTerrainValue;
    //}

}
