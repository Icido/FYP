using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCalculator : MonoBehaviour {

    public int mapSize;

    [Range(1f, 50f)]
    public float noiseScale;

    public float persistance;

    private List<Vector3> noiseMap;

    private List<Vector3> terrainMap = new List<Vector3>();

    public void UpdateTerrainMap(int popMapSize)
    {
        List<Vector3> tempPopMap = Noise.GenerateNoiseMap(mapSize, noiseScale);

        float terrainToMapScale = (float)mapSize / (float)popMapSize;

        if (tempPopMap != noiseMap)
        {
            noiseMap = tempPopMap;

            ResizeTerrainMap(noiseMap, terrainToMapScale);
        }
    }

    public void ResizeTerrainMap(List<Vector3> noiseMap, float terrainToMapScale)
    {
        terrainMap.Clear();

        for (int i = 0; i < noiseMap.Count; i++)
        {
            noiseMap[i] = new Vector3(noiseMap[i].x / terrainToMapScale, noiseMap[i].y * persistance, noiseMap[i].z / terrainToMapScale);
            terrainMap.Add(noiseMap[i]);
        }

    }
    
    public List<Vector3> getTerrainPoints()
    {
        return terrainMap;
    }


}
