using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredNearestNeighbours : MonoBehaviour {

    public int currentNumberOfConnections;
    public int maxNumberOfConnections;

    //Usage of dictionary to be able to request if the neighbour requested has been checks and found a path (from their point)
    public Dictionary<GameObject, bool> Neighbours = new Dictionary<GameObject, bool>();
}
