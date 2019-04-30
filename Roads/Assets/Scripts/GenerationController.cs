using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GenerationController : MonoBehaviour {

    public Button generateButton;

    public InputField terrMapSizeInputField;

    public InputField terrNoiseScaleInputField;

    public InputField terrAmplitudeInputField;

    public InputField popMapSizeInputField;

    public InputField popNoiseScaleInputField;

    public InputField highDensityLimitInputField;

    public InputField popAreaLimitInputField;

    public InputField seedInputField;

    public Toggle renderTerrainToggle;


    [SerializeField]
    private PopulationPoints popPoints;

    //Ensure terrainMapSize is never 0 or below
    private int terrainMapSize;

    [Range(1f, 50f)]
    private float terrainNoiseScale;

    private float amplitude;

    //Ensure populationMapSize is never 0 or below
    private int populationMapSize;

    [Range(0f, 1f)]
    private float highDensityLimit;

    [Range(1f, 50f)]
    private float populationNoiseScale;

    private int populationAreaSize;

    private int seed;

    private bool displayTerrain;

    private void Start()
    {
        generateButton.onClick.AddListener(beginGeneration);
    }

    void beginGeneration()
    {
        //Check all inputs are valid, then run
        //else return an error

        if(terrMapSizeInputField.text == null ||
            terrNoiseScaleInputField.text == null ||
            terrAmplitudeInputField.text == null ||
            popMapSizeInputField.text == null ||
            popNoiseScaleInputField.text == null ||
            highDensityLimitInputField.text == null ||
            popAreaLimitInputField.text == null)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        //Checks if the input values are the exact same as the previous input values
        //else return stating that it's already finished

        if (int.Parse(terrMapSizeInputField.text) == terrainMapSize &&
            float.Parse(terrNoiseScaleInputField.text) == terrainNoiseScale &&
            float.Parse(terrAmplitudeInputField.text) == amplitude &&
            int.Parse(popMapSizeInputField.text) == populationMapSize &&
            float.Parse(popNoiseScaleInputField.text) == populationNoiseScale &&
            float.Parse(highDensityLimitInputField.text) == highDensityLimit &&
            int.Parse(popAreaLimitInputField.text) == populationAreaSize &&
            int.Parse(seedInputField.text) == seed &&
            displayTerrain == renderTerrainToggle.isOn)
        {
            //Return as same previous inputs
            Debug.Log("Input already done!");
            return;
        }

        //Parse all text fields into usable values, then check to ensure they are valid values
        //else return an error

        terrainMapSize = int.Parse(terrMapSizeInputField.text);

        if(terrainMapSize <= 0)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        terrainNoiseScale = float.Parse(terrNoiseScaleInputField.text);

        if(terrainNoiseScale > 50f || terrainNoiseScale < 1f)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        amplitude = float.Parse(terrAmplitudeInputField.text);

        if(amplitude < 0f)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        populationMapSize = int.Parse(popMapSizeInputField.text);

        if(populationMapSize <= 0)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        populationNoiseScale = float.Parse(popNoiseScaleInputField.text);

        if(populationNoiseScale > 50f || populationNoiseScale < 1f)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        highDensityLimit = float.Parse(highDensityLimitInputField.text);

        if(highDensityLimit < 0f || highDensityLimit > 1f)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        populationAreaSize = int.Parse(popAreaLimitInputField.text);

        if(populationAreaSize < 0)
        {
            //Return error
            Debug.Log("Input incorrect!");
            return;
        }

        if (seedInputField.text == null)
        {
            seed = Random.Range(0, 1000);
        }
        else
        {
            seed = int.Parse(seedInputField.text);
        }

        displayTerrain = renderTerrainToggle.isOn;

        //Stopwatch st = new Stopwatch();

        //Debug.Log("Updating...");
        //st.Start();
        popPoints.updateLocations(terrainMapSize, terrainNoiseScale, amplitude, populationMapSize, highDensityLimit, populationNoiseScale, populationAreaSize, seed, displayTerrain);
        //st.Stop();
        //Debug.Log("Finished updating!");
        //Debug.Log("Update took " + st.ElapsedMilliseconds + " milliseconds to complete.");
        //Debug.Log("Total time: " + st.Elapsed);

        return;
    }
}
