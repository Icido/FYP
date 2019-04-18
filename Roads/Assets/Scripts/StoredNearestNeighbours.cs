using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredNearestNeighbours : MonoBehaviour {

    public int currentNumberOfConnections;
    public int maxNumberOfConnections;

    public Dictionary<GameObject, bool> Neighbours = new Dictionary<GameObject, bool>();
}
