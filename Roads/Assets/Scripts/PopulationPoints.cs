using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationPoints : MonoBehaviour {

    public PopulationCalculator noiseGeneration;

    public GameObject populationHubObject;

    public GameObject road;

    public List<GameObject> populationHotSpots = new List<GameObject>();

    public List<Vector2> popList = new List<Vector2>();


    public void updateLocations()
    {
     
        noiseGeneration.UpdateNoiseMap();
        
        //var tempList = noiseGeneration.getHighPopAreas();

        //if (checkLists(popList, tempList) == false)
        //{
        //    popList = tempList;

        popList = noiseGeneration.getHighPopAreas();
        hotspotGeneration(noiseGeneration.getHighPopAreas());

        NearestNeighbourFinder.roadConnections(populationHotSpots, noiseGeneration.mapSize);

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

    void hotspotGeneration(List<Vector2> hotspots)
    {
        if (populationHotSpots.Count > 0)
        {
            foreach (GameObject hotSpot in populationHotSpots)
            {
                Destroy(hotSpot);
            }

            populationHotSpots.Clear();
        }
        
        foreach (Vector2 location in popList)
        {
            Vector3 pos = new Vector3(location.x, 1, location.y);
            GameObject popHub = Instantiate(populationHubObject, pos, Quaternion.identity, transform);
            popHub.name = "Hotspot location " + popList.IndexOf(location);
            populationHotSpots.Add(popHub);
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
                newRoad.transform.Rotate(new Vector3(0, 90, 0));
                newRoad.name = "Road " + location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.IndexOf(point);
                newRoad.transform.localScale = new Vector3(distanceBetween, newRoad.transform.localScale.y, newRoad.transform.localScale.z);
                //Add to storage in StoredNearestNeighbours?


            }
        }



        return;
    }
}
