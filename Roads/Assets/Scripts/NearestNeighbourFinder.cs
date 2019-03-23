using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NearestNeighbourFinder {

    public static void roadConnections(List<GameObject> locations, float mapSize, int seed)
    {

        Random.InitState(seed);

        //TODO: Make seed a variable that can be changed and repeated to repeat roads as they were

        foreach(GameObject location in locations)
        {
            location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections = 0;

            location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections = Random.Range(0, 3) + Random.Range(0, 2) + 1;

            location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Clear();
        }

        foreach (GameObject location in locations)
        {
            int numRemainingConnections = location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections - location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections;

            if (numRemainingConnections <= 0)
                continue;

            float savedLoc1dist = new Vector3(-mapSize, -mapSize, -mapSize).sqrMagnitude;
            float savedLoc2dist = new Vector3(-mapSize, -mapSize, -mapSize).sqrMagnitude;
            float savedLoc3dist = new Vector3(-mapSize, -mapSize, -mapSize).sqrMagnitude;
            float savedLoc4dist = new Vector3(-mapSize, -mapSize, -mapSize).sqrMagnitude;

            int savedLoc1index = -1;
            int savedLoc2index = -1;
            int savedLoc3index = -1;
            int savedLoc4index = -1;

            int highestSavedLocation = -1;

            //Check against all other locations
            foreach (GameObject otherLocation in locations)
            {
                if (otherLocation == location || location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Contains(otherLocation))
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

            //Catches if 
            if (savedLoc1index == -1)
            {
                Debug.Log("No available connections for " + location.name);
                continue;
            }

            //Add connection between all locations and this location
            if (numRemainingConnections == 1 || (savedLoc2index == -1 && savedLoc3index == -1 && savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);

                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc1index]);
                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
            }
            else if(numRemainingConnections == 2 || (savedLoc3index == -1 && savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc1index]);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc2index]);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 2;
            }
            else if(numRemainingConnections == 3 || (savedLoc4index == -1))
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc1index]);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc2index]);

                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc3index]);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 3;
            }
            else
            {
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc1index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc1index]);

                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc2index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc2index]);

                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc3index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc3index]);

                locations[savedLoc4index].GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 1;
                locations[savedLoc4index].GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(location);
                location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.Add(locations[savedLoc4index]);

                location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections += 4;
            }
        }



        foreach (GameObject location in locations)
        {
            if (location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections - location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections > 0)
                Debug.Log("Empty slots available at location: " + location.name + ", number of slots: " + (location.GetComponent<StoredNearestNeighbours>().maxNumberOfConnections - location.GetComponent<StoredNearestNeighbours>().currentNumberOfConnections));

        }


    }

    static int findHighestDist(float dist1, float dist2, float dist3, float dist4)
    {
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



    //for each point in highdensityareas (HDA):
        //numConnections = random number between 1 and 4 (weighted toward 2/3)
        //for each HDA (except self):
            //find numConnections nearest neighbours via distance between, store locally
        //check all numconnections if they are already connected (each instanciated HDA stores list of GO of connected HDAs) and remove (as already connected)
        //instanciate road object connected between each, as a child of the initial connector



    //for each point in highdensityareas (HDA):
        //numConnections = random number between 1 and 4 (weighted toward 2/3)

    //for each point in highdensityareas (HDA):
        //for each HDA (except self):
            //check if numconnections of the chosen HDA has been filled
            //find numConnections nearest neighbours via distance between, store locally

        
    
        //check all numconnections if they are already connected (each instanciated HDA stores list of GO of connected HDAs) and remove (as already connected)
        //instanciate road object connected between each, as a child of the initial connector



}
