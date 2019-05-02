using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    //This noise class has two different ways of generating its data, as one is used to output a 2D float array and the other is as a List<Vector3>
    //The float[,] is for the terrain, as such utilises the amplitude to vary the height to a greater degree
    //The List<Vector3> is for the high density population locations, any points of Perlin Noise that is higher than the limit gets the location added

    public static List<Vector3> GenerateNoiseMapList(int mapSize, float scale, float highDensityLimit)
    {
        List<Vector3> noiseMap = new List<Vector3>();

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                if(perlinValue > highDensityLimit)
                    noiseMap.Add(new Vector3(x, 0, y));

            }
        }

        return noiseMap;

    }

    public static float[,] GenerateNoiseMapArray(int mapSize, float scale, float amplitude)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;


                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                noiseMap[x, y] = perlinValue * amplitude;
            }
        }

        return noiseMap;
    }

}
