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

    //Generates a float[x,z] and bool[x,z,neighbour] for the terrain given the input
    //The float[x,z] terrainMap is a 2D array of floats, the floats are received from the perlin noise and used as height
    //The bool[x,z,neighbour] terrainChecker is a 3D array of booleans, the first two iterators are given co-ordinates and the last iterator is of size 8, the number of neighbours surrounding the point
    //Each of these neighbours get their rise-over-run calculated and checked against the maximum rise-over-run. 
    //If they're too high (in either direction), the point returns false, else if it's okay it returns true.
    public static void UpdateTerrainMap(int mapSize, float noiseScale, float amplitude, float maxAngle)
    {
        terrainMapSize = mapSize;

        currentMaxAngle = maxAngle;

        //First the map is generated
        float[,] tempTerMap = Noise.GenerateNoiseMapArray(mapSize, noiseScale, amplitude);

        //If the map isn't equal to the previous map, then skip converting it into a bool[,,]
        if (tempTerMap != terrainMap)
        {
            terrainMap = tempTerMap;

            //Next generate the bool[,,] and cache for reference when checking neighbours in the A* pathfinding
            terrainConversion();
        }
    }

    private static void terrainConversion()
    {
        terrainChecker = new bool[terrainMapSize, terrainMapSize, 8];

        //Iterates over the terrain size, then checking each neighbour of that point
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

        //Since the neighbours would range from -1 to +1 on both the X and Y of the neighbour, iterates over each one (returning when it's centre square)
        //The neighbourNumber is iterated over, so that when iterating over the neighbours in A* they are the same neighbours, since they check in the same order
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

    //This checks the height and length between both the given point and its neighbour, and checking it against the maximum angle
    //Returning false if the angle between is too high in either direction, returning true if it's a valid value
    private static bool checkPoint(int x, int y, int neighbourX, int neighbourY)
    {
        if (neighbourX == terrainMapSize || neighbourX == -1 ||
            neighbourY == terrainMapSize || neighbourY == -1)
            return false;

        float dYHeight = Mathf.Abs(terrainMap[neighbourX, neighbourY] - terrainMap[x, y]);
        float dXLength = Vector2.Distance(new Vector2(neighbourX, neighbourY), new Vector2(x, y));
        float angleBetween = dYHeight / dXLength;

        if (angleBetween > currentMaxAngle)
            return false;


        return true;
    }

    //Returns terrain points
    public static float[,] getTerrainPoints()
    {
        return terrainMap;
    }

    //Returns terrain size
    public static int getTerrainSize()
    {
        return terrainMapSize;
    }

    //Returns terrain neighbour checker
    public static bool[,,] getTerrainChecker()
    {
        return terrainChecker;
    }

}
