using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NearestNeighbourFinder {

    public static void roadConnections(List<GameObject> locations, int seed)
    {
        //Sets the seed to the given seed, useful for repeating the same neighbours and values if necessary
        Random.InitState(seed);

        //Resets each stored nearest connections, clears them and gives them a new maximum number of connections
        foreach(GameObject location in locations)
        {
            location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections = 0;

            location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections = Random.Range(0, 3) + Random.Range(0, 2) + 1;

            location.GetComponent<StoredNearestNeighbours>().Neighbours.Clear();
        }

        //This checks each location against all other locations to search for the nearest neighbours (up to their maximum amount)
        foreach (GameObject location in locations)
        {
            //Firstly checks how many available connections remain, as some could have already connected to this particular location prior to checking and cannot find any more new connections
            int numRemainingConnections = location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections - location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections;

            if (numRemainingConnections <= 0)
                continue;

            //Temporary storage of the saved location distances and indexes
            float savedLoc1dist = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue).sqrMagnitude;
            float savedLoc2dist = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue).sqrMagnitude;
            float savedLoc3dist = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue).sqrMagnitude;
            float savedLoc4dist = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue).sqrMagnitude;

            int savedLoc1index = -1;
            int savedLoc2index = -1;
            int savedLoc3index = -1;
            int savedLoc4index = -1;

            int highestSavedLocation = -1;

            //Check against all other locations
            foreach (GameObject otherLocation in locations)
            {
                if (otherLocation == location || location.GetComponent<StoredNearestNeighbours>().Neighbours.ContainsKey(otherLocation))
                    continue;

                if (otherLocation.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections == otherLocation.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections)
                    continue;

                float distBetween = (location.transform.position - otherLocation.transform.position).sqrMagnitude;
                
                highestSavedLocation = findHighestDist(savedLoc1dist, savedLoc2dist, savedLoc3dist, savedLoc4dist);

                //Check against the highest distance currently saved
                switch (highestSavedLocation)
                {
                    case 1:
                        if(distBetween < savedLoc1dist)
                        {
                            savedLoc1index = locations.IndexOf(otherLocation);
                            savedLoc1dist = distBetween;
                        }

                        break;

                    case 2:
                        if (distBetween < savedLoc2dist)
                        {
                            savedLoc2index = locations.IndexOf(otherLocation);
                            savedLoc2dist = distBetween;
                        }

                        break;

                    case 3:
                        if (distBetween < savedLoc3dist)
                        {
                            savedLoc3index = locations.IndexOf(otherLocation);
                            savedLoc3dist = distBetween;
                        }

                        break;

                    case 4:
                        if (distBetween < savedLoc4dist)
                        {
                            savedLoc4index = locations.IndexOf(otherLocation);
                            savedLoc4dist = distBetween;
                        }

                        break;
                }

            }

            //Catches if there are no available connections, if so then move on
            if (savedLoc1index == -1)
            {
                //Debug.Log("No available connections for " + location.name);
                continue;
            }

            //Add connection between all locations and this location
            if (numRemainingConnections == 1 || (savedLoc2index == -1 && savedLoc3index == -1 && savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc1index], false);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;

            }
            else if(numRemainingConnections == 2 || (savedLoc3index == -1 && savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc1index], false);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc2index], false);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 2;
            }
            else if(numRemainingConnections == 3 || (savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc1index], false);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc2index], false);

                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc3index], false);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 3;
            }
            else
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc1index], false);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc2index], false);

                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc3index], false);

                locations[savedLoc4index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc4index].GetComponent<StoredNearestNeighbours>().Neighbours.Add(location, false);
                location.GetComponent<StoredNearestNeighbours>().Neighbours.Add(locations[savedLoc4index], false);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 4;
            }
        }
    }

    //Searches over the 4 stored distances and returns the highest distance index
    static int findHighestDist(float dist1, float dist2, float dist3, float dist4)
    {
        //Simply gets the maximum distance of dist 1, dist 2, dist 3 and dist 4
        float maxDist = Mathf.Max(dist1, Mathf.Max(dist2, Mathf.Max(dist3, dist4)));

        if (maxDist == dist1)
            return 1;

        if (maxDist == dist2)
            return 2;

        if (maxDist == dist3)
            return 3;

        if (maxDist == dist4)
            return 4;

        //Default
        return 1;
    }

}
