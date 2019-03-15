using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationPoints : MonoBehaviour {

    public PopulationCalculator noiseGeneration;

    public GameObject populationHubObject;

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

        NearestNeighbourFinder.roadConnections(populationHotSpots);
        

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
                Object.Destroy(hotSpot);
            }

            populationHotSpots.Clear();
        }
        
        foreach (Vector2 location in popList)
        {
            Vector3 pos = new Vector3(location.x, 1, location.y);
            GameObject popHub = Instantiate(populationHubObject, pos, Quaternion.identity, transform);
            populationHotSpots.Add(popHub);
        }

        return;
    }
}
