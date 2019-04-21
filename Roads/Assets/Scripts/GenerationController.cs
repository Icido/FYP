using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationController : MonoBehaviour
{

    public PopulationPoints popPoints;

    private bool isWorking = false;

    //Thread on how not to freeze main thread - look for coroutine section
    //https://gamedev.stackexchange.com/questions/113096/how-to-not-freeze-the-main-thread-in-unity

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !isWorking)
        {
            Debug.Log("Updating...");
            isWorking = popPoints.isWorking;
            StartCoroutine(popPoints.updatingLocations());
            isWorking = popPoints.isWorking;
            //popPoints.updateLocations();
            Debug.Log("Finished updating!");
        }

        if (Input.GetKey(KeyCode.Return))
        {
            Debug.Log("Paused!");
            Debug.Break();
        }
    }
}
