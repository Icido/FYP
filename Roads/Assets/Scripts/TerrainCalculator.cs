using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCalculator : MonoBehaviour {

    public int mapSize;

    [Range(1f, 50f)]
    public float noiseScale;

    private List<Vector3> noiseMap;

    public void UpdateTerrainMap()
    {
        List<Vector3> tempPopMap = Noise.GenerateNoiseMap(mapSize, noiseScale);

        if (tempPopMap != noiseMap)
        {
            noiseMap = tempPopMap;

            //DrawNoiseMap(noiseMap);
        }
    }

}
