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

    private int maxNumChecks = 50;
    private int currentNumChecks = 0;

    private float currentMaxAngle;
    private float maxAngle = 45f; // Should have a max angle of ~15f-20f degrees of steepness, 45f is for testing purposes only!
                                  // The steepest streets in England range from 21.81 degrees (Vale Street, Bristol) to 16.09 degrees (Gold Hill, Shaftesbury, Dorset)
                                  // https://www.bbc.co.uk/news/uk-england-38568893, https://ichef.bbci.co.uk/news/624/cpsprodpb/0BC6/production/_95241030_englands-steepest-streets-6-2.png

    //What causes the hang-time is that the A* algorithm cannot find a clear path from start to finish. Either changing the max angle or chaning the terrain amplitude solves this.
    //There must be a better solution for the A* algorithm to find a clearer path.
    //(Perhaps make a check if the road takes too long to incease the max angle by a small margin until it finds an appropriate path.

    // if(road is taking too long && time between last check and this check is long enough)
    //      maxAngle *= 1.15f;
    
    public List<Vector2Int> roadConnections(Vector3 startPoint, Vector3 finishPoint, float[,] terrainPoints)
    {
        neighbours.Clear();

        Point start = new Point(startPoint);
        Point finish = new Point(finishPoint);

        currentNumChecks = 0;
        currentMaxAngle = maxAngle;


        //TODO: Figure out how the dictionaries affect further road creation.
        closedSet.Clear();
        openSet.Clear();
        gScore.Clear();
        fScore.Clear();
        nodeLinks.Clear();

        Debug.Log("Initial fScore from " + startPoint + " to " + finishPoint + ": " + Heuristic(start, finish));

        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = Heuristic(start, finish);

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
            currentNumChecks++;

            if (openSet.Count > terrainPoints.Length && currentNumChecks >= maxNumChecks)
            {
                Debug.Log("Can't find  road from " + startPoint + " to " + finishPoint + "!");
                Debug.Log("Increasing max road angle to attempt to open a path...");
                currentMaxAngle *= 1.15f;
                Debug.Log("New max road angle: " + currentMaxAngle + " degrees.");
                currentNumChecks = 0;
            }

            openSet.Remove(current);
            closedSet[current] = true;

            neighbours.Clear();
            neighbours = get8Neighbours(terrainPoints, current);
            

            foreach(var neighbour in neighbours)
            {
                if (closedSet.ContainsKey(neighbour))
                    continue;

                int projectedG = getGScore(current) + 1;

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
        int dx = finish.X - start.X;
        int dy = finish.Y - start.Y;
        //float dHeight = finish.height - start.height;
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

    private bool isValidNeighbour(float[,] terrainPoints, Point point, Point centralPoint)
    {
        if (point.X < 0 || point.X >= terrainPoints.Length)
            return false;

        if (point.Y < 0 || point.Y >= terrainPoints.Length)
            return false;

        float dYHeight = Mathf.Abs(point.height - centralPoint.height);
        float dXLength = Vector2Int.Distance(new Vector2Int(point.X, point.Y), new Vector2Int(centralPoint.X, centralPoint.Y));
        float angleBetween = Mathf.Atan(dYHeight / dXLength) * Mathf.Rad2Deg;

        //Checks if the angle is too step between this point and the central point
        if (angleBetween >= currentMaxAngle)
            return false;


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

    public bool isSamePoint(Point point)
    {
        if (X != point.X)
            return false;

        if (Y != point.Y)
            return false;

        return true;
    }
}

