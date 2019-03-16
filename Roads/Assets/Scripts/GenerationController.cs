using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationController : MonoBehaviour {

    public PopulationPoints popPoints;

    // Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Updating...");
            popPoints.updateLocations();
            Debug.Log("Finished updating!");
        }
	}
}
