using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PopulationPoints))]
public class GenerationController : MonoBehaviour {

    //Input fields
    public InputField terrMapSizeInputField;
    public InputField terrNoiseScaleInputField;
    public InputField terrAmplitudeInputField;
    public InputField popMapSizeInputField;
    public InputField popNoiseScaleInputField;
    public InputField highDensityLimitInputField;
    public InputField popAreaLimitInputField;
    public InputField seedInputField;
    public Toggle renderTerrainToggle;


    //Buttons
    public Button generateButton;
    public Button showHideButton;
    public Button closeButton;


    //UI displays
    public RectTransform UIGroup;

    public GameObject WarningGroup;
    private RectTransform WarningRect;
    private Text WarningText;


    //UI Lerp values
    private float UItimeOfTravel = 0.25f;
    private float UIcurrentTime = 0f;
    private float UInormalizedValue;
    private Vector3 UIstartPosition;
    private Vector3 UIendPosition;
    private bool isHidden = false;


    //Warning Lerp values
    private float WarningTimeOfTravel = 0.25f;
    private float WarningCurrentTime = 0f;
    private float WarningNormalizedValue;
    private Vector3 WarningStartPosition;
    private Vector3 WarningEndPosition;


    //Reference to the script on this gameObject
    private PopulationPoints popPoints;


    //Generation values (stored for repetition prevention purposes)
    private int terrainMapSize;
    private float terrainNoiseScale;
    private float amplitude;
    private int populationMapSize;
    private float highDensityLimit;
    private float populationNoiseScale;
    private int populationAreaSize;
    private int seed;
    private bool displayTerrain;


    private void Start()
    {
        popPoints = GetComponent<PopulationPoints>();

        generateButton.onClick.AddListener(beginGeneration);
        showHideButton.onClick.AddListener(showHideWrapper);
        closeButton.onClick.AddListener(closeProgram);
    }

    void beginGeneration()
    {
        //Check all inputs are valid, then run
        //else return an error

        if(terrMapSizeInputField.text == "" ||
            terrNoiseScaleInputField.text == "" ||
            terrAmplitudeInputField.text == "" ||
            popMapSizeInputField.text == "" ||
            popNoiseScaleInputField.text == "" ||
            highDensityLimitInputField.text == "" ||
            popAreaLimitInputField.text == "")
        {
            StartCoroutine(errorShow(1));
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
            StartCoroutine(errorShow(2));
            return;
        }

        //Parse all text fields into usable values, then check to ensure they are valid values
        //else return an error

        terrainMapSize = int.Parse(terrMapSizeInputField.text);

        if(terrainMapSize <= 0)
        {
            StartCoroutine(errorShow(3));
            return;
        }

        terrainNoiseScale = float.Parse(terrNoiseScaleInputField.text);

        if(terrainNoiseScale > 50f)
        {
            StartCoroutine(errorShow(4));
            return;
        }

        if (terrainNoiseScale < 1f)
        {
            StartCoroutine(errorShow(5));
            return;
        }

        amplitude = float.Parse(terrAmplitudeInputField.text);

        if(amplitude < 0f)
        {
            StartCoroutine(errorShow(6));
            return;
        }

        populationMapSize = int.Parse(popMapSizeInputField.text);

        if(populationMapSize <= 0)
        {
            StartCoroutine(errorShow(7));
            return;
        }

        populationNoiseScale = float.Parse(popNoiseScaleInputField.text);

        if(populationNoiseScale > 50f)
        {
            StartCoroutine(errorShow(8));
            return;
        }

        if(populationNoiseScale < 1f)
        {
            StartCoroutine(errorShow(9));
            return;
        }

        highDensityLimit = float.Parse(highDensityLimitInputField.text);

        if(highDensityLimit < 0f)
        {
            StartCoroutine(errorShow(10));
            return;
        }

        if(highDensityLimit > 1f)
        {
            StartCoroutine(errorShow(11));
            return;
        }

        populationAreaSize = int.Parse(popAreaLimitInputField.text);

        if(populationAreaSize < 0)
        {
            StartCoroutine(errorShow(12));
            return;
        }

        if (seedInputField.text == "")
        {
            seed = Random.Range(0, 1000);
        }
        else
        {
            seed = int.Parse(seedInputField.text);
        }

        displayTerrain = renderTerrainToggle.isOn;

        StartCoroutine(popPoints.updateLocsCoroutine(terrainMapSize, terrainNoiseScale, amplitude, populationMapSize, highDensityLimit, populationNoiseScale, populationAreaSize, seed, displayTerrain));

        return;
    }

    void showHideWrapper()
    {
        StartCoroutine(showHide());
    }

    IEnumerator showHide()
    {
        UIstartPosition = new Vector3(UIGroup.anchoredPosition.x, UIGroup.anchoredPosition.y, 0f);

        UIcurrentTime = 0f;

        if (!isHidden)
        {
            UIendPosition = new Vector3(UIGroup.anchoredPosition.x + 400f, UIGroup.anchoredPosition.y, 0f);
            showHideButton.GetComponentInChildren<Text>().text = "<";
        }
        else
        {
            UIendPosition = new Vector3(UIGroup.anchoredPosition.x - 400f, UIGroup.anchoredPosition.y, 0f);
            showHideButton.GetComponentInChildren<Text>().text = ">";
        }


        while (UIcurrentTime <= UItimeOfTravel)
        {
            UIcurrentTime += Time.deltaTime;
            UInormalizedValue = UIcurrentTime / UItimeOfTravel;

            UIGroup.anchoredPosition = Vector3.Lerp(UIstartPosition, UIendPosition, UInormalizedValue);
            yield return null;
        }

        isHidden = !isHidden;
    }

    IEnumerator errorShow(int errorCase)
    {
        string errorString = "";

        GameObject newWarning = Instantiate(WarningGroup, FindObjectOfType<Canvas>().transform);
        newWarning.SetActive(true);
        WarningRect = newWarning.GetComponent<RectTransform>();
        WarningText = newWarning.GetComponentInChildren<Text>();

        switch (errorCase)
        {
            case 1:
                errorString = "Invalid value entry. Please revise inputs.";
                break;
            case 2:
                errorString = "Inputs same as previous inputs!";
                break;
            case 3:
                errorString = "Terrain Map Size too low! Please keep it above 0.";
                break;
            case 4:
                errorString = "Terrain Noise Scale too high! Please keep it below 50.";
                break;
            case 5:
                errorString = "Terrain Noise Scale too low! Please keep it above 1.";
                break;
            case 6:
                errorString = "Terrain Amplitude too low! Please keep it above 0.";
                break;
            case 7:
                errorString = "Population Map Size too low! Please keep it above 0.";
                break;
            case 8:
                errorString = "Population Noise Scale too high! Please keep it below 50.";
                break;
            case 9:
                errorString = "Population Noise Scale too low! Please keep it above 0.";
                break;
            case 10:
                errorString = "High Density Limit too low! Please keep it above 0.";
                break;
            case 11:
                errorString = "High Density Limit too high! Please keep it below 1.";
                break;
            case 12:
                errorString = "Population Area Limit too low! Please keep it above 0.";
                break;
        }

        WarningText.GetComponent<Text>().text = errorString;

        //Disable button while message is displayed
        generateButton.interactable = false;


        //Show loop

        WarningStartPosition = new Vector3(WarningRect.anchoredPosition.x, WarningRect.anchoredPosition.y, 0f);
        WarningEndPosition = new Vector3(WarningRect.anchoredPosition.x, WarningRect.anchoredPosition.y + 100f, 0f);

        WarningCurrentTime = 0f;

        while (WarningCurrentTime <= WarningTimeOfTravel)
        {
            WarningCurrentTime += Time.deltaTime;
            WarningNormalizedValue = WarningCurrentTime / WarningTimeOfTravel;

            WarningRect.anchoredPosition = Vector3.Lerp(WarningStartPosition, WarningEndPosition, WarningNormalizedValue);
            yield return null;
        }
        
        //Wait for display
        yield return new WaitForSeconds(5);

        //Reenable button after displaying errormes
        generateButton.interactable = true;


        //Hide loop

        WarningStartPosition = new Vector3(WarningRect.anchoredPosition.x, WarningRect.anchoredPosition.y, 0f);
        WarningEndPosition = new Vector3(WarningRect.anchoredPosition.x, WarningRect.anchoredPosition.y - 100f, 0f);


        WarningCurrentTime = 0f;

        while (WarningCurrentTime <= WarningTimeOfTravel)
        {
            WarningCurrentTime += Time.deltaTime;
            WarningNormalizedValue = WarningCurrentTime / WarningTimeOfTravel;

            WarningRect.anchoredPosition = Vector3.Lerp(WarningStartPosition, WarningEndPosition, WarningNormalizedValue);
            yield return null;
        }

        Destroy(newWarning);

    }

    //Program exit routine
    void closeProgram()
    {
        popPoints.appKill();
        StopAllCoroutines();
        Application.Quit();
    }

}
