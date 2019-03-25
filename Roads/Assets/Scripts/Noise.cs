using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static List<Vector3> GenerateNoiseMapList(int mapSize, float scale)
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
                noiseMap.Add(new Vector3(x, perlinValue, y));
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
