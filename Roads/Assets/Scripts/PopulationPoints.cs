using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationPoints : MonoBehaviour {

    public PopulationCalculator populationGeneration;

    public TerrainCalculator terrainGeneration;

    public GameObject populationHubObject;

    public GameObject road;

    public List<GameObject> populationHotSpots = new List<GameObject>();

    public List<GameObject> terrainSpots = new List<GameObject>();

    public List<Vector3> popList = new List<Vector3>();

    private List<Vector2> openQueue = new List<Vector2>();
    private List<Vector2> closedQueue = new List<Vector2>();

    private GameObject popSpots;

    private GameObject terSpots;


    public void updateLocations()
    {

        terrainGeneration.UpdateTerrainMap(populationGeneration.mapSize);

        populationGeneration.UpdatePopulationMap(terrainGeneration.getTerrainPoints());
        
        //var tempList = noiseGeneration.getHighPopAreas();

        //if (checkLists(popList, tempList) == false)
        //{
        //    popList = tempList;

        popList = populationGeneration.getHighPopAreas();
        hotspotGeneration(populationGeneration.getHighPopAreas());

        terrainSpotGeneration(terrainGeneration.getTerrainPoints());
        
        NearestNeighbourFinder.roadConnections(populationHotSpots, populationGeneration.mapSize, populationGeneration.seed);

        roadGeneration(populationHotSpots);

        //}

        return;
    }

    bool checkLists(List<Vector2> list1, List<Vector2> list2)
    {
        if ((list1 == null) && (list2 == null))
            return true;
        else if ((list1 == null) || (list2 == null))
            return false;

        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }

        return true;
    }

    void hotspotGeneration(List<Vector3> hotspots)
    {

        if(GameObject.Find("Population Points") == null)
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
            popHub.name = "Hotspot location " + popList.IndexOf(location);
            populationHotSpots.Add(popHub);
        }

        return;
    }

    void terrainSpotGeneration(float[,] terrainPoints)
    {
        if (GameObject.Find("Terrain Points") == null)
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
                GameObject terSpot = Instantiate(populationHubObject, new Vector3(x, terrainPoints[x,y], y), Quaternion.identity, terSpots.transform);
                terSpot.name = "Terrain spot " + (y * terrainPoints.GetLength(1) + x);
                terrainSpots.Add(terSpot);
            }
        }

        terSpots.SetActive(false);

        return;
    }

    void roadGeneration(List<GameObject> locations)
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



        foreach (GameObject location in locations)
        {
            foreach (GameObject point in location.GetComponent<StoredNearestNeighbours>().ConnectedLocations)
            {
                closedQueue.Clear();
                openQueue.Clear();

                roadConnections(point.transform.position, location.transform.position, terrainGeneration.getTerrainPoints());
                
                Vector3 midPoint = new Vector3((point.transform.position.x + location.transform.position.x) / 2,
                                               (point.transform.position.y + location.transform.position.y) / 2,
                                               (point.transform.position.z + location.transform.position.z) / 2);

                float distanceBetween = Vector3.Distance(location.transform.position, point.transform.position);
                GameObject newRoad = Instantiate(road, midPoint, Quaternion.identity, location.transform);
                newRoad.transform.LookAt(point.transform);
                newRoad.name = "Road " + location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.IndexOf(point);
                newRoad.transform.localScale = new Vector3(newRoad.transform.localScale.x, newRoad.transform.localScale.y, distanceBetween);
                //Add to storage in StoredNearestNeighbours?


            }
        }

        return;
    }

    void roadConnections(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints)
    {
        //Find distance between start and finish
        Vector3 groundedStartPoint = new Vector3(startPoint.x, 0, startPoint.z);
        Vector3 groundedFinishPoint = new Vector3(finishPoint.x, 0, finishPoint.z);

        float linearDistance = (groundedFinishPoint - groundedStartPoint).sqrMagnitude;
        //Debug.Log("Linear distance " + linearDistance);

        float heightDistance = startPoint.y - finishPoint.y;
        //Debug.Log("Height distance " + heightDistance);

        //int maxEdge = terrainPoints.Length - 1;
        //getAdjacentLocations(maxEdge, point);



        //Vector3 tempStartPoint = startPoint;
        //movetoward finish point, then scout for other roads
        //if there's another road, recalculate toward it and find the closest point on that road to finishPoint



        return;
    }

    List<Vector2> getAdjacentLocations(int maxTerrainEdge, Vector2 point)
    {
        List<Vector2> adjacentLocations = new List<Vector2>();

        bool isLeftHorizontalEdge = false;
        bool isRightHorizontalEdge = false;

        bool isTopVerticleEdge = false;
        bool isBottomVerticalEdge = false;

        if(point.x == 0)
        {
            isLeftHorizontalEdge = true;
        }
        else if(point.x == maxTerrainEdge)
        {
            isRightHorizontalEdge = true;
        }

        if(point.y == 0)
        {
            isBottomVerticalEdge = true;
        }
        else if(point.y == maxTerrainEdge)
        {
            isTopVerticleEdge = true;
        }

        if(isTopVerticleEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x, point.y - 1));

            return adjacentLocations;
        }
        else if(isTopVerticleEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x, point.y - 1));

            return adjacentLocations;
        }
        else if(isBottomVerticalEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));

            return adjacentLocations;
        }
        else if(isBottomVerticalEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));

            return adjacentLocations;
        }
        else if(isTopVerticleEdge)
        {
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));

            return adjacentLocations;
        }
        else if(isBottomVerticalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));

            return adjacentLocations;
        }
        else if(isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x, point.y - 1));

            return adjacentLocations;
        }
        else if(isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x, point.y - 1));

            return adjacentLocations;
        }
        else
        {
            adjacentLocations.Add(new Vector2(point.x, point.y + 1));

            adjacentLocations.Add(new Vector2(point.x + 1, point.y + 1));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y));
            adjacentLocations.Add(new Vector2(point.x + 1, point.y - 1));

            adjacentLocations.Add(new Vector2(point.x, point.y - 1));

            adjacentLocations.Add(new Vector2(point.x - 1, point.y - 1));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y));
            adjacentLocations.Add(new Vector2(point.x - 1, point.y + 1));

            return adjacentLocations;
        }
    }
}
