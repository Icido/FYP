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

    private float maxAngle = 45f;

    private float riseOverRunAngle;

    private AStarPathfinding aStar = new AStarPathfinding();

    //private long previousElapsedMilliseconds = 0;

    public void updateLocations(int terrainMapSize, float noiseScale, float amplitude, int populationMapSize, float highDensityLimit, float populationNoiseScale, int populationAreaSize, int seed, bool displayTerrain)
    {
        //Stopwatch st = new Stopwatch();
        //previousElapsedMilliseconds = 0;
        //st.Start();

        riseOverRunAngle = Mathf.Tan(maxAngle);

        TerrainCalculator.UpdateTerrainMap(terrainMapSize, noiseScale, amplitude, riseOverRunAngle);
        //Debug.Log("Finished Terrain generation");
        //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
        //Debug.Log("UpdateTerrainMap took " + previousElapsedMilliseconds + " milliseconds to complete.");

        terrainPoints = TerrainCalculator.getTerrainPoints();

        PopulationCalculator.UpdatePopulationMap(terrainPoints, populationMapSize, highDensityLimit, populationNoiseScale, populationAreaSize);
        //Debug.Log("Finished Population generation");
        //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
        //Debug.Log("UpdatePopulationMap took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        hotspotGeneration(PopulationCalculator.getHighPopAreas());
        //Debug.Log("Finished Hotspot generation");
        //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
        //Debug.Log("HotspotGeneration took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        if (displayTerrain)
        {
            terrainSpotGeneration(terrainPoints);
            //Debug.Log("Finished TerrainPoint generation");
            //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
            //Debug.Log("TerrainSpotGeneration took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        }

        NearestNeighbourFinder.roadConnections(populationHotSpots, seed);
        //Debug.Log("Finished finding nearest neighbours");
        //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
        //Debug.Log("RoadConnections took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        roadGeneration(populationHotSpots, terrainPoints);
        //Debug.Log("Finished Road generation");
        //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
        //Debug.Log("RoadGeneration took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        //st.Stop();

        return;
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

        //terSpots.SetActive(false);

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
    
    void roadGeneration(List<GameObject> locations, float[,] terrPoints)
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

        //Stopwatch st = new Stopwatch();
        //st.Start();
        foreach (GameObject location in locations)
        {

            //Create a temporary list of keys to iterate over to prevent iteration errors while in the foreach loop
            List<GameObject> keys = new List<GameObject>(location.GetComponent<StoredNearestNeighbours>().Neighbours.Keys);

            int roadNum = 0;

            foreach (GameObject point in keys)
            {
                if (location.GetComponent<StoredNearestNeighbours>().Neighbours[point] == true ||
                    point.GetComponent<StoredNearestNeighbours>().Neighbours[location] == true)
                    continue;

                List<Vector2Int> roadConnectionsList = new List<Vector2Int>();

                roadConnectionsList.Clear();

                //StartCoroutine(aStarConnections(point, location, terrPoints));

                roadConnectionsList = aStar.roadConnections(point.transform.position, location.transform.position, terrPoints, terrainChecker, riseOverRunAngle);
                roadConnectionsList.Insert(0, new Vector2Int((int)point.transform.position.x, (int)point.transform.position.z)); ;
                //Debug.Log("Road from " + point.transform.position + " to " + location.transform.position + " has " + roadConnectionsList.Count + " connection points between");

                for (int i = 0; i < roadConnectionsList.Count - 1; i++)
                {
                    Vector3 startPosition = new Vector3(roadConnectionsList[i].x, terrPoints[roadConnectionsList[i].x, roadConnectionsList[i].y], roadConnectionsList[i].y);

                    Vector3 endPosition = new Vector3(roadConnectionsList[i + 1].x, terrPoints[roadConnectionsList[i + 1].x, roadConnectionsList[i + 1].y], roadConnectionsList[i + 1].y);

                    Vector3 midPoint = new Vector3((startPosition.x + endPosition.x) / 2, (startPosition.y + endPosition.y) / 2, (startPosition.z + endPosition.z) / 2);

                    float distanceBetween = Vector3.Distance(startPosition, endPosition);
                    GameObject newRoad = Instantiate(roadObject, midPoint, Quaternion.identity, location.transform);
                    newRoad.transform.LookAt(endPosition);
                    newRoad.name = "Road " + (i + 1);
                    newRoad.transform.localScale = new Vector3(newRoad.transform.localScale.x, newRoad.transform.localScale.y, distanceBetween);
                }

                roadNum++;

                location.GetComponent<StoredNearestNeighbours>().Neighbours[point] = true;
                locations.Find(o => o == point).GetComponent<StoredNearestNeighbours>().Neighbours[location] = true;
            }

            //previousElapsedMilliseconds = st.ElapsedMilliseconds - previousElapsedMilliseconds;
            //Debug.Log(roadNum + " roads from point " + location.transform.position + " took " + st.ElapsedMilliseconds + " milliseconds to complete.");

        }

        //st.Stop();

        return;
    }

    IEnumerator aStarConnections(GameObject point, GameObject location, float[,] terrPoints)
    {

        yield return null;

    }



}
