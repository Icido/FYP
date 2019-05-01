using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PopulationPoints : MonoBehaviour {

    public GameObject populationHubObject;

    public GameObject terrainObject;

    public GameObject roadObject;

    private List<GameObject> populationHotSpots = new List<GameObject>();

    private List<GameObject> terrainSpots = new List<GameObject>();

    private GameObject popSpots;

    private GameObject terSpots;

    private float[,] terrainPoints;

    //The rate of rise would be 2 meters in 10 meters, 2 over 10. This would be the maximum incline along these roads.
    private float riseOverRun = 0.2f;

    public IEnumerator updateLocsCoroutine(int terrainMapSize, float noiseScale, float amplitude, int populationMapSize, float highDensityLimit, float populationNoiseScale, int populationAreaSize, int seed, bool displayTerrain)
    {

        TerrainCalculator.UpdateTerrainMap(terrainMapSize, noiseScale, amplitude, riseOverRun);

        terrainPoints = TerrainCalculator.getTerrainPoints();

        PopulationCalculator.UpdatePopulationMap(terrainPoints, populationMapSize, highDensityLimit, populationNoiseScale, populationAreaSize);

        hotspotGeneration(PopulationCalculator.getHighPopAreas());

        if (displayTerrain)
        {
            terrainSpotGeneration(terrainPoints);
        }
        else if(terrainSpots.Count > 0)
        {
            foreach (GameObject terrainSpot in terrainSpots)
            {
                Destroy(terrainSpot);
            }

            terrainSpots.Clear();
        }

        NearestNeighbourFinder.roadConnections(populationHotSpots, seed);

        StartCoroutine(roadGeneration(populationHotSpots, terrainPoints));

        yield return null;
    }

    void terrainSpotGeneration(float[,] terrainPoints)
    {

        //if (GameObject.Find("Terrain Points") == null)
        if (terSpots == null)
        {
            terSpots = new GameObject();
            terSpots.name = "Terrain Points";
            terSpots.transform.position = Vector3.zero;
        }

        if (terrainSpots.Count > 0)
        {
            foreach (GameObject terrainSpot in terrainSpots)
            {
                Destroy(terrainSpot);
            }

            terrainSpots.Clear();
        }

        for (int y = 0; y < terrainPoints.GetLength(1); y++)
        {
            for (int x = 0; x < terrainPoints.GetLength(0); x++)
            {
                GameObject terSpot = Instantiate(terrainObject, new Vector3(x, terrainPoints[x,y], y), Quaternion.identity, terSpots.transform);
                terSpot.name = "Terrain spot " + (y * terrainPoints.GetUpperBound(0) + x);
                terrainSpots.Add(terSpot);
            }
        }

        return;
    }

    void hotspotGeneration(List<Vector3> hotspots)
    {

        //if(GameObject.Find("Population Points") == null)
        if(popSpots == null)
        {
            popSpots = new GameObject();
            popSpots.name = "Population Points";
            popSpots.transform.position = Vector3.zero;
        }

        if (populationHotSpots.Count > 0)
        {
            foreach (GameObject hotSpot in populationHotSpots)
            {
                Destroy(hotSpot);
            }

            populationHotSpots.Clear();
        }
        
        foreach (Vector3 location in hotspots)
        {
            GameObject popHub = Instantiate(populationHubObject, location, Quaternion.identity, popSpots.transform);
            popHub.name = "Hotspot location " + hotspots.IndexOf(location);
            populationHotSpots.Add(popHub);
        }

        if (!popSpots.activeInHierarchy)
            popSpots.SetActive(true);

        return;
    }
    
    IEnumerator roadGeneration(List<GameObject> locations, float[,] terrPoints)
    {
        foreach (GameObject loc in locations)
        {
            int numChildren = loc.transform.childCount;

            if (numChildren > 0)
            {
                for (int i = 0; i < numChildren; i++)
                {
                    Destroy(loc.transform.GetChild(i).gameObject);
                }
            }
        }

        bool[,,] terrainChecker = TerrainCalculator.getTerrainChecker();

        foreach (GameObject location in locations)
        {
            //Create a temporary list of keys to iterate over to prevent iteration errors while in the foreach loop
            List<GameObject> keys = new List<GameObject>(location.GetComponent<StoredNearestNeighbours>().Neighbours.Keys);

            foreach (GameObject point in keys)
            {
                if (location.GetComponent<StoredNearestNeighbours>().Neighbours[point] == true ||
                    point.GetComponent<StoredNearestNeighbours>().Neighbours[location] == true)
                    continue;

                location.GetComponent<StoredNearestNeighbours>().Neighbours[point] = true;
                locations.Find(o => o == point).GetComponent<StoredNearestNeighbours>().Neighbours[location] = true;

                StartCoroutine(aStarConnections(point.transform.position, location, terrPoints, terrainChecker, riseOverRun));

                yield return null;
            }
        }
    }

    IEnumerator aStarConnections(Vector3 point, GameObject location, float[,] terrPoints, bool[,,] terrChecker, float maxAngle)
    {

        //Stopwatch st = new Stopwatch();
        //st.Start();
        //st.Stop();
        //Debug.Log("EuclideanHeuristic took:" + st.ElapsedMilliseconds + " milliseconds");

        List<Vector3> roadConnectionsList = new List<Vector3>();
        AStarPathfinding aStar = new AStarPathfinding();

        Vector3 startPosition = new Vector3();
        Vector3 endPosition = new Vector3();
        Vector3 midPoint = new Vector3();
        float distanceBetween;

        roadConnectionsList.Clear();

        yield return StartCoroutine(aStar.runCoroutine(point, location.transform.position, terrPoints, terrChecker, riseOverRun));

        roadConnectionsList = aStar.newRoad;
        roadConnectionsList.Insert(0, new Vector3(point.x, point.y, point.z));

        for (int i = 0; i < roadConnectionsList.Count - 1; i++)
        {
            startPosition = roadConnectionsList[i];

            endPosition = roadConnectionsList[i + 1];

            midPoint = new Vector3((startPosition.x + endPosition.x) / 2, (startPosition.y + endPosition.y) / 2, (startPosition.z + endPosition.z) / 2);

            distanceBetween = Vector3.Distance(startPosition, endPosition);
            GameObject newRoad = Instantiate(roadObject, midPoint, Quaternion.identity, location.transform);
            newRoad.transform.LookAt(endPosition);
            newRoad.name = "Road " + (i + 1);
            newRoad.transform.localScale = new Vector3(newRoad.transform.localScale.x, newRoad.transform.localScale.y, distanceBetween);

            yield return null;
        }

    }
}
