using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainCalculator {

    private static float[,] terrainMap;

    private static bool[,,] terrainChecker;

    private static int terrainMapSize;

    private static int currentNeighbourX;

    private static int currentNeighbourY;

    private static float currentMaxAngle;

    public static void UpdateTerrainMap(int mapSize, float noiseScale, float amplitude, float maxAngle)
    {
        terrainMapSize = mapSize;

        currentMaxAngle = maxAngle;

        float[,] tempTerMap = Noise.GenerateNoiseMapArray(mapSize, noiseScale, amplitude);

        if (tempTerMap != terrainMap)
        {
            terrainMap = tempTerMap;

            terrainConversion();
        }
    }

    public static float[,] getTerrainPoints()
    {
        return terrainMap;
    }

    public static int getTerrainSize()
    {
        return terrainMapSize;
    }

    private static void terrainConversion()
    {
        terrainChecker = new bool[terrainMapSize, terrainMapSize, 8];

        for (int y = 0; y < terrainMapSize; y++)
        {
            for (int x = 0; x < terrainMapSize; x++)
            {
                neighbourIteration(x, y);
            }
        }

        return;
    }

    private static void neighbourIteration(int x, int y)
    {
        int neighbourNumber = 0;

        for (int neighbourY = -1; neighbourY < 2; neighbourY++)
        {
            for (int neighbourX = -1; neighbourX < 2; neighbourX++)
            {
                if (neighbourX == 0 && neighbourY == 0)
                    continue;

                currentNeighbourX = x + neighbourX;
                currentNeighbourY = y + neighbourY;

                terrainChecker[x, y, neighbourNumber] = checkPoint(x, y, currentNeighbourX, currentNeighbourY);

                neighbourNumber++;
                
            }
        }
    }

    private static bool checkPoint(int x, int y, int neighbourX, int neighbourY)
    {
        if (neighbourX == terrainMapSize || neighbourX == -1 ||
            neighbourY == terrainMapSize || neighbourY == -1)
            return false;

        float dYHeight = Mathf.Abs(terrainMap[neighbourX, neighbourY] - terrainMap[x, y]);
        float dXLength = Vector2.Distance(new Vector2(neighbourX, neighbourY), new Vector2(x, y));
        float angleBetween = dYHeight / dXLength;

        if (angleBetween >= currentMaxAngle)
            return false;


        return true;
    }

    public static bool[,,] getTerrainChecker()
    {
        return terrainChecker;
    }

}
