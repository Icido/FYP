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

    private List<Vector2Int> roadConnectionsList = new List<Vector2Int>();

    private GameObject popSpots;

    private GameObject terSpots;

    private float[,] terrainPoints;

    private AStarPathfinding aStar = new AStarPathfinding();


    public void updateLocations()
    {
        
        terrainGeneration.UpdateTerrainMap(); //TODO: Inverse mapToTerrain the other way, so that the map maps to the terrain instead
        //Debug.Log("Finished Terrain generation");
        
        terrainPoints = terrainGeneration.getTerrainPoints();

        populationGeneration.UpdatePopulationMap(terrainPoints);
        //Debug.Log("Finished Population generation");
        
        popList = populationGeneration.getHighPopAreas();
        hotspotGeneration(populationGeneration.getHighPopAreas());
        //Debug.Log("Finished Hotspot generation");

        //terrainSpotGeneration(terrainPoints);
        //Debug.Log("Finished TerrainPoint generation");

        NearestNeighbourFinder.roadConnections(populationHotSpots, populationGeneration.mapSize, populationGeneration.seed);
        //Debug.Log("Finished finding nearest neighbours");

        //roadGeneration(populationHotSpots, terrainPoints);
        //Debug.Log("Finished Road generation");

        return;
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

        if (GameObject.Find("Terrain Points") == terSpots)
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

        foreach (GameObject location in locations)
        {
            foreach (GameObject point in location.GetComponent<StoredNearestNeighbours>().ConnectedLocations)
            {
                roadConnectionsList.Clear();
                roadConnectionsList = aStar.roadConnections(point.transform.position, location.transform.position, terrPoints);

                for (int i = 0; i < roadConnectionsList.Count - 1; i++)
                {
                    Vector3 startPosition = new Vector3(roadConnectionsList[i].x, terrPoints[roadConnectionsList[i].x, roadConnectionsList[i].y], roadConnectionsList[i].y);

                    Vector3 endPosition = new Vector3(roadConnectionsList[i + 1].x, terrPoints[roadConnectionsList[i + 1].x, roadConnectionsList[i + 1].y], roadConnectionsList[i + 1].y);

                    Vector3 midPoint = new Vector3((startPosition.x + endPosition.x) / 2, (startPosition.y + endPosition.y) / 2, (startPosition.z + endPosition.z) / 2);

                    float distanceBetween = Vector3.Distance(startPosition, endPosition);
                    GameObject newRoad = Instantiate(road, midPoint, Quaternion.identity, location.transform);
                    newRoad.transform.LookAt(point.transform);
                    newRoad.name = "Road " + location.GetComponent<StoredNearestNeighbours>().ConnectedLocations.IndexOf(point);
                    newRoad.transform.localScale = new Vector3(newRoad.transform.localScale.x, newRoad.transform.localScale.y, distanceBetween);
                    //Add to storage in StoredNearestNeighbours?
                }
            }
        }

        return;
    }



    List<Vector2Int> get8AdjacentLocations(int maxTerrainEdge, Vector2Int Vector2Int, Vector2Int parentVector2Int)
    {
        List<Vector2Int> adjacentLocations = new List<Vector2Int>();

        bool isLeftHorizontalEdge = false;
        bool isRightHorizontalEdge = false;

        bool isTopVerticleEdge = false;
        bool isBottomVerticalEdge = false;

        if (Vector2Int.x == 0)
        {
            isLeftHorizontalEdge = true;
        }
        else if (Vector2Int.x == maxTerrainEdge)
        {
            isRightHorizontalEdge = true;
        }

        if (Vector2Int.y == 0)
        {
            isBottomVerticalEdge = true;
        }
        else if (Vector2Int.y == maxTerrainEdge)
        {
            isTopVerticleEdge = true;
        }

        if (isTopVerticleEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isTopVerticleEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isTopVerticleEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y - 1));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y + 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
    }

    List<Vector2Int> get4AdjacentLocations(int maxTerrainEdge, Vector2Int Vector2Int, Vector2Int parentVector2Int)
    {
        List<Vector2Int> adjacentLocations = new List<Vector2Int>();

        bool isLeftHorizontalEdge = false;
        bool isRightHorizontalEdge = false;

        bool isTopVerticleEdge = false;
        bool isBottomVerticalEdge = false;

        if (Vector2Int.x == 0)
        {
            isLeftHorizontalEdge = true;
        }
        else if (Vector2Int.x == maxTerrainEdge)
        {
            isRightHorizontalEdge = true;
        }

        if (Vector2Int.y == 0)
        {
            isBottomVerticalEdge = true;
        }
        else if (Vector2Int.y == maxTerrainEdge)
        {
            isTopVerticleEdge = true;
        }

        if (isTopVerticleEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isTopVerticleEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge && isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge && isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isTopVerticleEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isBottomVerticalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isLeftHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else if (isRightHorizontalEdge)
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
        else
        {
            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y + 1));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x + 1, Vector2Int.y));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x, Vector2Int.y - 1));

            adjacentLocations.Add(new Vector2Int(Vector2Int.x - 1, Vector2Int.y));

            adjacentLocations.Remove(parentVector2Int);

            return adjacentLocations;
        }
    }


}
