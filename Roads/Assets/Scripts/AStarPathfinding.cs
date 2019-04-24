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

    private float maxAngle = 45f; // Should have a max angle of ~15f-20f degrees of steepness, 45f is for testing purposes only!
                                  // The steepest street gradients in England range from 21.81 degrees (Vale Street, Bristol) to 16.09 degrees (Gold Hill, Shaftesbury, Dorset)
                                  // https://www.bbc.co.uk/news/uk-england-38568893, https://ichef.bbci.co.uk/news/624/cpsprodpb/0BC6/production/_95241030_englands-steepest-streets-6-2.png

    //private float diagonalCost = Mathf.Sqrt(2);
    //private float normalCost = 1f;
    private float stepCost = 1f;

    //What causes the hang-time is that the A* algorithm cannot find a clear path from start to finish. Either changing the max angle or chaning the terrain amplitude solves this.
    //There must be a better solution for the A* algorithm to find a clearer path.

    //Paper for better and more efficient deadlock failsafes (if deadlock becomes a problem again)
    //https://www.aaai.org/Papers/ICAPS/2008/ICAPS08-047.pdf

    public List<Vector2Int> roadConnections(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints)
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

        Debug.Log("Initial fScore from " + startPoint + " to " + finishPoint + ": " + EuclideanHeuristic(start, finish));

        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = EuclideanHeuristic(start, finish);

        int counter = 0;

        while(openSet.Count > 0)
        {
            Point current = nextBest();

            if (current.isSamePoint(finish))
            {
                Debug.Log("Finished finding road from " + startPoint + " to " + finishPoint + " in " + counter + " steps");
                return reconstruction(current);
            }

            counter++;

            openSet.Remove(current);
            closedSet[current] = true;

            neighbours.Clear();
            neighbours = get8Neighbours(terrainPoints, current);
            

            foreach(var neighbour in neighbours)
            {
                if (closedSet.ContainsKey(neighbour))
                    continue;

                float projectedG;

                projectedG = getGScore(current) + stepCost + (getAngleBetween(current, neighbour) / 45f);

                //if(neighbour.isDiagonal == true)
                //    projectedG = Mathf.Pow(getGScore(current) + diagonalCost + (getAngleBetween(current, neighbour) / 45f), 2);
                //else
                //    projectedG = Mathf.Pow(getGScore(current) + normalCost + (getAngleBetween(current, neighbour) / 45f), 2);

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

    private List<Point> get8Neighbours(float[,] terrainPoints, Point center)
    {
        List<Point> newNeighbours = new List<Point>();

        int centralX = center.X;
        int centralY = center.Y;

        int terrUpper = terrainPoints.GetUpperBound(0);

        Point pt = new Point(center);

        //Bottom row
        if (centralX > 0 && centralY > 0)
        {
            pt = new Point(centralX - 1, centralY - 1, terrainPoints[centralX - 1, centralY - 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        if (centralY > 0)
        {
            pt = new Point(centralX, centralY - 1, terrainPoints[centralX, centralY - 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        if (centralX < terrUpper && centralY > 0)
        {
            pt = new Point(centralX + 1, centralY - 1, terrainPoints[centralX + 1, centralY - 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        //Middle row
        if (centralX > 0)
        {
            pt = new Point(centralX - 1, centralY, terrainPoints[centralX - 1, centralY]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        if (centralX < terrUpper)
        {
            pt = new Point(centralX + 1, centralY, terrainPoints[centralX + 1, centralY]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        //Top row
        if (centralX > 0 && centralY < terrUpper)
        {
            pt = new Point(centralX - 1, centralY + 1, terrainPoints[centralX - 1, centralY + 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        if (centralY < terrUpper)
        {
            pt = new Point(centralX, centralY + 1, terrainPoints[centralX, centralY + 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        if (centralX < terrUpper && centralY < terrUpper)
        {
            pt = new Point(centralX + 1, centralY + 1, terrainPoints[centralX + 1, centralY + 1]);
            if (isValidNeighbour(terrainPoints, pt, center))
                newNeighbours.Add(pt);
        }

        //if (newNeighbours.Count <= 1)
        //{
        //    Debug.LogError("Not enough neighbours! Could not find suitable neighbour, causing deadlock and no path to be produced.");
        //    //Debug.Break();
        //}

        return newNeighbours;
    }

    private bool isValidNeighbour(float[,] terrainPoints, Point point, Point centralPoint)
    {
        if (point.X < 0 || point.X >= terrainPoints.Length)
            return false;

        if (point.Y < 0 || point.Y >= terrainPoints.Length)
            return false;

        float angleBetween = getAngleBetween(centralPoint, point);

        //Checks if the angle is too step between this point and the central point
        if (angleBetween >= maxAngle)
            return false;


        return true;
    }

    private float getAngleBetween(Point current, Point neighbour)
    {
        float dYHeight = Mathf.Abs(neighbour.height - current.height);
        float dXLength = Vector2.Distance(new Vector2(neighbour.X, neighbour.Y), new Vector2(current.X, current.Y));
        float angleBetween = Mathf.Atan(dYHeight / dXLength) * Mathf.Rad2Deg;

        return angleBetween;
    }

}
#endregion

public struct Point
{
    public int X, Y;

    public float height;

    //public bool isDiagonal = false;

    public Point(Point p) { X = p.X; Y = p.Y; height = p.height; }
    public Point(int x, int y, float h) { X = x; Y = y; height = h; }
    public Point(Vector3 v3) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; }
    public Point(Vector2 v2, float h) { X = (int)v2.x; Y = (int)v2.y; height = h; }
    public Point(Vector2Int v2, float h) { X = v2.x; Y = v2.y; height = h; }

    //public Point(int x, int y, float h, bool diagonal) { X = x; Y = y; height = h; isDiagonal = diagonal; }
    //public Point(Vector3 v3, bool diagonal) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; isDiagonal = diagonal; }
    //public Point(Vector2 v2, float h, bool diagonal) { X = (int)v2.x; Y = (int)v2.y; height = h; isDiagonal = diagonal; }
    //public Point(Vector2Int v2, float h, bool diagonal) { X = v2.x; Y = v2.y; height = h; isDiagonal = diagonal; }


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

