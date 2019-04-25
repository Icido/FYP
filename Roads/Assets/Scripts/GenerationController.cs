using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GenerationController : MonoBehaviour {

    public PopulationPoints popPoints;

    //Ensure terrainMapSize is 0
    public int terrainMapSize;

    [Range(1f, 50f)]
    public float terrainNoiseScale;

    public float amplitude;

    //Ensure mapSize is never 0
    public int populationMapSize;

    [Range(0f, 1f)]
    public float highDensityLimit = 0.95f;

    [Range(1f, 50f)]
    public float populationNoiseScale;

    public int populationAreaSize;

    public int seed;

    public bool displayTerrain = false;

    // Update is called once per frame
    void Update () {
        

		if(Input.GetKeyUp(KeyCode.Space))
        {
            Stopwatch st = new Stopwatch();

            Debug.Log("Updating...");
            st.Start();
            popPoints.updateLocations(terrainMapSize, terrainNoiseScale, amplitude, populationMapSize, highDensityLimit, populationNoiseScale, populationAreaSize, seed, displayTerrain);
            st.Stop();
            Debug.Log("Finished updating!");
            Debug.Log("Update took " + st.ElapsedMilliseconds + " milliseconds to complete.");
            Debug.Log("Total time: " + st.Elapsed);
        }
	}
}
