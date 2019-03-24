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

    void terrainSpotGeneration(List<Vector3> terrainPoints)
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

        foreach (Vector3 point in terrainPoints)
        {
            GameObject terSpot = Instantiate(populationHubObject, point, Quaternion.identity, terSpots.transform);
            terSpot.name = "Terrain spot " + terrainPoints.IndexOf(point);
            terrainSpots.Add(terSpot);
        }



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
}
