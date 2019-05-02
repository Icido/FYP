using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class AStarPathfinding {

    Dictionary<Point, bool> closedSet = new Dictionary<Point, bool>();
    Dictionary<Point, bool> openSet = new Dictionary<Point, bool>();

    //Cost of start to THIS point
    Dictionary<Point, float> gScore = new Dictionary<Point, float>();

    //Cost of start to goal through THIS point
    Dictionary<Point, float> fScore = new Dictionary<Point, float>();

    Dictionary<Point, Point> nodeLinks = new Dictionary<Point, Point>();

    List<Point> neighbours = new List<Point>();

    public List<Vector3> newRoad = new List<Vector3>();

    private float diagonalCost = Mathf.Sqrt(2);
    private float normalCost = 1f;

    //Maximum riseOverRun (set to 1 over the given value for ease of using as a multiplication when used in getAngledCost)
    private float maxAngle;

    public IEnumerator runPathfinding(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints, bool[,,] terrainChecker, float riseOverRun)
    {
        neighbours.Clear();

        maxAngle = 1 / riseOverRun;

        Point start = new Point(startPoint);
        Point finish = new Point(finishPoint);

        closedSet.Clear();
        openSet.Clear();
        gScore.Clear();
        fScore.Clear();
        nodeLinks.Clear();

        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = EuclideanHeuristic(start, finish);

        int counter = 0;

        while (openSet.Count > 0)
        {
            Point current = nextBest();

            if (current.isSamePoint(finish))
            {
                newRoad = reconstruction(current, terrainPoints);
                yield break;
            }

            if (counter > terrainPoints.Length)
            {
                Debug.LogError("Unable to find a route in a suitable length of time from " + startPoint + " to " + finishPoint);
                break;
            }

            counter++;

            openSet.Remove(current);
            closedSet[current] = true;

            neighbours.Clear();
            getNeighbours(terrainPoints, terrainChecker, current);


            foreach (var neighbour in neighbours)
            {
                if (closedSet.ContainsKey(neighbour))
                    continue;

                float projectedG;

                if (neighbour.isDiagonal == true)
                    projectedG = getGScore(current) + diagonalCost + getAngledCost(current, neighbour);
                else
                    projectedG = getGScore(current) + normalCost + getAngledCost(current, neighbour);

                if (!openSet.ContainsKey(neighbour))
                    openSet[neighbour] = true;
                else if (projectedG >= getGScore(neighbour))
                    continue;

                nodeLinks[neighbour] = current;
                gScore[neighbour] = projectedG;
                fScore[neighbour] = projectedG + EuclideanHeuristic(neighbour, finish);
            }

            //Using yield return null waits until the end of the update, therefore making the process much longer, if it even actually completes
            //yield return null;

        }
        yield break;
    }

    #region HelperFunctions

    //Finds the heuristic distance from the start to the finish
    //TODO: return the squared result and modify the algorithm to take this into account
    private float EuclideanHeuristic(Point start, Point finish)
    {
        float dx = Mathf.Pow(finish.X - start.X, 2);
        float dy = Mathf.Pow(finish.Y - start.Y, 2);
        float dHeight = Mathf.Pow(finish.height - start.height, 2);
        return Mathf.Sqrt(dx + dy + dHeight);
    }

    //Searches the openSet to find the lowest f-score to pathfind from
    private Point nextBest()
    {
        float best = float.MaxValue;
        Point bestPoint = new Point();
        foreach(var node in openSet.Keys)
        {
            var score = getFScore(node);
            if(score < best)
            {
                bestPoint = node;
                best = score;
            }
        }

        return bestPoint;
    }

    //Returns f-score of a given point
    private float getFScore(Point node)
    {
        float score = float.MaxValue;
        fScore.TryGetValue(node, out score);
        return score;
    }

    //Returns g-score of a given point
    private float getGScore(Point node)
    {
        float score = float.MaxValue;
        gScore.TryGetValue(node, out score);
        return score;
    }

    //Traverses back through the nodeLinks dictionary to convert into a list of vector3 Points to use for instantiating the road
    private List<Vector3> reconstruction(Point current, float[,] terrPoints)
    {
        List<Vector3> path = new List<Vector3>();
        while(nodeLinks.ContainsKey(current))
        {
            path.Add(new Vector3(current.X, terrPoints[current.X, current.Y], current.Y));
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

    //Checks through the 8 surrounding neighbours and adds any that return a true value from terrainChecker
    //TerrainChecker is a cached 3D array that stores all 8 surrounding points of a given point on the terrain
    //If any of these points gives a greater slope than the maximum grade (in either direction), it is returned false and will never be traversed
    private void getNeighbours(float[,] terrainPoints, bool[,,] terrainChecker, Point center)
    {
        int centralX = center.X;
        int centralY = center.Y;

        Point pt = new Point();

        for (int i = 0; i < 8; i++)
        {
            if (terrainChecker[centralX, centralY, i])
            {
                switch (i)
                {
                    case 0:
                        pt = new Point(centralX - 1, centralY - 1, terrainPoints[centralX - 1, centralY - 1], true);
                        neighbours.Add(pt);
                        break;
                    case 1:
                        pt = new Point(centralX, centralY - 1, terrainPoints[centralX, centralY - 1], false);
                        neighbours.Add(pt);
                        break;
                    case 2:
                        pt = new Point(centralX + 1, centralY - 1, terrainPoints[centralX + 1, centralY - 1], true);
                        neighbours.Add(pt);
                        break;
                    case 3:
                        pt = new Point(centralX - 1, centralY, terrainPoints[centralX - 1, centralY], false);
                        neighbours.Add(pt);
                        break;
                    case 4:
                        pt = new Point(centralX + 1, centralY, terrainPoints[centralX + 1, centralY], false);
                        neighbours.Add(pt);
                        break;
                    case 5:
                        pt = new Point(centralX - 1, centralY + 1, terrainPoints[centralX - 1, centralY + 1], true);
                        neighbours.Add(pt);
                        break;
                    case 6:
                        pt = new Point(centralX, centralY + 1, terrainPoints[centralX, centralY + 1], false);
                        neighbours.Add(pt);
                        break;
                    case 7:
                        pt = new Point(centralX + 1, centralY + 1, terrainPoints[centralX + 1, centralY + 1], true);
                        neighbours.Add(pt);
                        break;
                }
            }
        }


        return;
    }

    //This function takes two points and returns on a 0-1 scale how close to the maximum the grade is
    //The steeper the angle, the closer to the grade, the higher the cost to traverse (set in the g-score)
    private float getAngledCost(Point current, Point neighbour)
    {
        float dYHeight = Mathf.Abs(neighbour.height - current.height);
        float dXLength = Vector2.Distance(new Vector2(neighbour.X, neighbour.Y), new Vector2(current.X, current.Y));
        float angleBetween = (dYHeight / dXLength) * maxAngle;

        return angleBetween;
    }

    #endregion

}

//Point as a struct for storing these values in a relatively simple container
//the isDiagonal bool is only ever used in checking if the neighbouring point is of a diagonal relative to the central point, upping its g-score cost to get to.
public struct Point
{
    public int X, Y;

    public float height;

    public bool isDiagonal;

    public Point(int x, int y, float h, bool diagonal) { X = x; Y = y; height = h; isDiagonal = diagonal; }
    public Point(Vector3 v3) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; isDiagonal = false; }

    //Function to ensure the two points are the same in terms of their co-ordinates, regardless of other values
    public bool isSamePoint(Point point)
    {
        if (X != point.X)
            return false;

        if (Y != point.Y)
            return false;

        return true;
    }
}

//This KeyComparer replaces the .Equals functionality in Dictionary comparisons of the struct Point
//Instead of checking if either are exactly the same, it only checks their x and y co-ordinate and heights
class KeyComparer : IEqualityComparer<Point>
{
    public bool Equals(Point x, Point y)
    {
        return (x.X == y.X) && (x.Y == y.Y) && (x.height == y.height);
    }

    public int GetHashCode(Point obj)
    {
        return obj.GetHashCode();
    }
}

