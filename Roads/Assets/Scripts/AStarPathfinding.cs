using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding {

    Dictionary<Point, bool> closedSet = new Dictionary<Point, bool>();
    Dictionary<Point, bool> openSet = new Dictionary<Point, bool>();

    //Cost of start to THIS point
    Dictionary<Point, int> gScore = new Dictionary<Point, int>();

    //Cost of start to goal through THIS point
    Dictionary<Point, float> fScore = new Dictionary<Point, float>();

    Dictionary<Point, Point> nodeLinks = new Dictionary<Point, Point>();

    List<Point> neighbours = new List<Point>();

    private float maxAngle = 45f;


    public List<Vector2Int> roadConnections(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints)
    {
        neighbours.Clear();

        Point start = new Point(startPoint);
        Point finish = new Point(finishPoint);

        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = Heuristic(start, finish);

        while(openSet.Count > 0)
        {
            var current = nextBest();
            if(current.Equals(finish))
            {
                return reconstruction(current);
            }

            if (openSet.Count > (terrainPoints.Length * terrainPoints.Length))
            {
                Debug.Log("Can't find road!");
                Debug.Break();
                Debug.DebugBreak();
            }

            openSet.Remove(current);
            closedSet[current] = true;

            neighbours.Clear();
            neighbours = get8Neighbours(terrainPoints, current);
            

            foreach(var neighbour in neighbours)
            {
                if (closedSet.ContainsKey(neighbour))
                    continue;

                var projectedG = getGScore(current) + 1;

                if (!openSet.ContainsKey(neighbour))
                    openSet[neighbour] = true;
                else if (projectedG >= getGScore(neighbour))
                    continue;


                nodeLinks[neighbour] = current;
                gScore[neighbour] = projectedG;
                fScore[neighbour] = projectedG + Heuristic(neighbour, finish);// + Mathf.Abs(terrainPoints[neighbour.X, neighbour.Y] - terrainPoints[current.X, current.Y]);
            }
        }

        return new List<Vector2Int>();
    }

    private int Heuristic(Point start, Point finish)
    {
        var dx = finish.X - start.X;
        var dy = finish.Y - start.Y;
        //var dHeight = finish.height - start.height;
        return Mathf.Abs(dx) + Mathf.Abs(dy);// + Mathf.Abs(dHeight);
    }

    private Point nextBest()
    {
        float best = float.MaxValue;
        Point bestPoint = null;
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

    private int getGScore(Point node)
    {
        int score = int.MaxValue;
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

        //Bottom row
        Point pt = new Point(center.X - 1, center.Y - 1, terrainPoints[center.X - 1, center.Y - 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        pt = new Point(center.X, center.Y - 1, terrainPoints[center.X, center.Y - 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        pt = new Point(center.X + 1, center.Y - 1, terrainPoints[center.X + 1, center.Y - 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        //Middle row
        pt = new Point(center.X - 1, center.Y, terrainPoints[center.X - 1, center.Y]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        pt = new Point(center.X + 1, center.Y, terrainPoints[center.X + 1, center.Y]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        //Top row
        pt = new Point(center.X - 1, center.Y + 1, terrainPoints[center.X - 1, center.Y + 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        pt = new Point(center.X, center.Y + 1, terrainPoints[center.X, center.Y + 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        pt = new Point(center.X + 1, center.Y + 1, terrainPoints[center.X + 1, center.Y + 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        if (newNeighbours.Count <= 1)
        {
            Debug.LogError("Not enough neighbours! Could not find suitable neighbour, causing deadlock and no path to be produced.");
            Debug.Break();
            Debug.DebugBreak();
        }

        return newNeighbours;
    }

    private List<Point> get4Neighbours(float[,] terrainPoints, Point center)
    {
        List<Point> newNeighbours = new List<Point>();

        //Bottom
        Point pt = new Point(center.X, center.Y - 1, terrainPoints[center.X, center.Y - 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        //Left
        pt = new Point(center.X - 1, center.Y, terrainPoints[center.X - 1, center.Y]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        //Right
        pt = new Point(center.X + 1, center.Y, terrainPoints[center.X + 1, center.Y]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        //Top
        pt = new Point(center.X, center.Y + 1, terrainPoints[center.X, center.Y + 1]);
        if (isValidNeighbour(terrainPoints, pt, center))
            newNeighbours.Add(pt);

        if (newNeighbours.Count <= 1)
        {
            Debug.LogError("Not enough neighbours! Could not find suitable neighbour, causing deadlock and no path to be produced.");
            Debug.Break();
            Debug.DebugBreak();
        }

        return newNeighbours;
    }


    private bool isValidNeighbour(float[,] terrainPoints, Point point, Point centralPoint)
    {
        if (point.X < 0 || point.X >= terrainPoints.Length)
            return false;

        if (point.Y < 0 || point.Y >= terrainPoints.Length)
            return false;

        //float dYHeight = Mathf.Abs(point.height - centralPoint.height);
        //float dXLength = Vector2Int.Distance(new Vector2Int(point.X, point.Y), new Vector2Int(centralPoint.X, centralPoint.Y));
        //float angleBetween = Mathf.Atan(dYHeight / dXLength) * Mathf.Rad2Deg;

        ////Checks if the angle is too step between this point and the central point
        //if (angleBetween > maxAngle)
        //    return false;


        return true;
    }

}


public class Point
{
    public int X, Y;

    public float height;

    public Point(int x, int y, float h) { X = x; Y = y; height = h; }
    public Point(Vector3 v3) { X = (int)v3.x; Y = (int)v3.z; height = v3.y; }
    public Point(Vector2 v2, float h) { X = (int)v2.x; Y = (int)v2.y; height = h; }
    public Point(Vector2Int v2, float h) { X = v2.x; Y = v2.y; height = h; }
}