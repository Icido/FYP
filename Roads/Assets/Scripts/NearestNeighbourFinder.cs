using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NearestNeighbourFinder {

    public static void roadConnections(List<GameObject> locations)
    {
        foreach (GameObject location in locations)
        {
            int numConnections = Random.Range(0, 3) + Random.Range(0, 2) + 1;

            List<GameObject> connectedSpots = new List<GameObject>();

            //Check against all other locations
            foreach (GameObject otherLocation in locations)
            {
                if (otherLocation == location)
                    continue;

                if(connectedSpots.Count < numConnections)
                {
                    connectedSpots.Add(otherLocation);
                    continue;
                }

                float distBetween = (location.transform.position - otherLocation.transform.position).sqrMagnitude;

                GameObject locationToBeRemoved = new GameObject();

                bool checkAgainstOthers = false;

                foreach (GameObject storedLocation in connectedSpots)
                {
                    //If one object has been flagged to be farther away than new point, all other points will be checked if they are farther than the first checked point
                    if(checkAgainstOthers)
                    {
                        if((location.transform.position - storedLocation.transform.position).sqrMagnitude > (location.transform.position - locationToBeRemoved.transform.position).sqrMagnitude)
                        {
                            locationToBeRemoved = storedLocation;
                        }
                        
                        continue;
                    }

                    //Checking if new location is smaller than any of the stored locations
                    if((location.transform.position - storedLocation.transform.position).sqrMagnitude > distBetween)
                    {
                        locationToBeRemoved = storedLocation;
                        checkAgainstOthers = true;
                    }
                }

                connectedSpots.Remove(locationToBeRemoved);
                connectedSpots.Add(location);

            }

            location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.AddRange(connectedSpots);
            location.GetComponent<StoredNearestNeighbours>().numberOfConnections = numConnections;
            location.GetComponent<StoredNearestNeighbours>().numberOfActualConnectedLocations = connectedSpots.Count;

            //Instanciate roads along connections




        }
    }

    //for each point in highdensityareas (HDA):
        //numConnections = random number between 1 and 4 (weighted toward 2/3)
        //for each HDA (except self):
            //find numConnections nearest neighbours via distance between, store locally
        //check all numconnections if they are already connected (each instanciated HDA stores list of GO of connected HDAs) and remove (as already connected)
        //instanciate road object connected between each, as a child of the initial connector





}
