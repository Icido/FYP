using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding {

    Dictionary<Point, bool> closedSet = new Dictionary<Point, bool>();
    Dictionary<Point, bool> openSet = new Dictionary<Point, bool>();

    //Cost of start to THIS point
    Dictionary<Point, float> gScore = new Dictionary<Point, float>();

    //Cost of start to goal through THIS point
    Dictionary<Point, float> fScore = new Dictionary<Point, float>();

    Dictionary<Point, Point> nodeLinks = new Dictionary<Point, Point>();

    List<Point> neighbours = new List<Point>();

    private float diagonalCost = Mathf.Sqrt(2);
    private float normalCost = 1f;
    //private float stepCost = 1f;

    //What causes the hang-time is that the A* algorithm cannot find a clear path from start to finish. Either changing the max angle or chaning the terrain amplitude solves this.
    //There must be a better solution for the A* algorithm to find a clearer path.

    //Paper for better and more efficient deadlock failsafes (if deadlock becomes a problem again)
    //https://www.aaai.org/Papers/ICAPS/2008/ICAPS08-047.pdf

    public List<Vector2Int> roadConnections(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints, bool[,,] terrainChecker)
    {
        neighbours.Clear();

        Point start = new Point(startPoint);
        Point finish = new Point(finishPoint);

        //TODO: Figure out how the dictionaries affect further road creation.
        closedSet.Clear();
        openSet.Clear();
        gScore.Clear();
        fScore.Clear();
        nodeLinks.Clear();

        //Debug.Log("Initial fScore from " + startPoint + " to " + finishPoint + ": " + EuclideanHeuristic(start, finish));

        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = EuclideanHeuristic(start, finish);

        int counter = 0;

        while(openSet.Count > 0)
        {
            Point current = nextBest();

            if (current.isSamePoint(finish))
            {
                //Debug.Log("Finished finding road from " + startPoint + " to " + finishPoint + " in " + counter + " steps");
                return reconstruction(current);
            }

            if(counter > terrainPoints.Length)
            {
                Debug.LogError("Unable to find a route in a suitable length of time from " + startPoint + " to " + finishPoint);
                break;
            }

            counter++;

            openSet.Remove(current);
            closedSet[current] = true;

            neighbours.Clear();
            getNeighbours(terrainPoints, terrainChecker, current);
            

            foreach(var neighbour in neighbours)
            {
                if (closedSet.ContainsKey(neighbour))
                    continue;

                float projectedG;

                //projectedG = getGScore(current) + stepCost + (getAngleBetween(current, neighbour) / 45f);

                if (neighbour.isDiagonal == true)
                    projectedG = getGScore(current) + diagonalCost;// + getAngledCost(current, neighbour);
                else
                    projectedG = getGScore(current) + normalCost;// + getAngledCost(current, neighbour);

                if (!openSet.ContainsKey(neighbour))
                    openSet[neighbour] = true;
                else if (projectedG >= getGScore(neighbour))
                    continue;

                nodeLinks[neighbour] = current;
                gScore[neighbour] = projectedG;
                fScore[neighbour] = projectedG + EuclideanHeuristic(neighbour, finish);
            }
        }

        return new List<Vector2Int>();
    }

    #region HelperFunctions

    private float EuclideanHeuristic(Point start, Point finish)
    {
        float dx = Mathf.Pow(finish.X - start.X, 2);
        float dy = Mathf.Pow(finish.Y - start.Y, 2);
        float dHeight = Mathf.Pow(finish.height - start.height, 2);
        return Mathf.Sqrt(dx + dy + dHeight);

    }

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

    private float getFScore(Point node)
    {
        float score = float.MaxValue;
        fScore.TryGetValue(node, out score);
        return score;
    }

    private float getGScore(Point node)
    {
        float score = float.MaxValue;
        gScore.TryGetValue(node, out score);
        return score;
    }

    private List<Vector2Int> reconstruction(Point current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while(nodeLinks.ContainsKey(current))
        {
            path.Add(new Vector2Int(current.X, current.Y));
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

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


    private float getAngledCost(Point current, Point neighbour)
    {
        float dYHeight = Mathf.Abs(neighbour.height - current.height);
        float dXLength = Vector2.Distance(new Vector2(neighbour.X, neighbour.Y), new Vector2(current.X, current.Y));
        float angleBetween = (dYHeight / dXLength) * Mathf.Rad2Deg;

        return angleBetween;
    }

}
#endregion

public struct Point
{
    public int X, Y;

    public float height;

    //public float movementCost;

    public bool isDiagonal;

    public Point(int x, int y, float h, bool diagonal) { X = x; Y = y; height = h; isDiagonal = diagonal; }
    public Point(Vector3 v3) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; isDiagonal = false; }

    //public Point(int x, int y, float h, float mC, bool diagonal) { X = x; Y = y; height = h; movementCost = mC; isDiagonal = diagonal; }
    //public Point(Vector3 v3) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; movementCost = 0f; isDiagonal = false; }



    public bool isSamePoint(Point point)
    {
        if (X != point.X)
            return false;

        if (Y != point.Y)
            return false;

        return true;
    }
}

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

