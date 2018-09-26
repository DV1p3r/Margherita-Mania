using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookerManager : MonoBehaviour {

    public float F_CookTime = 3f;
    public float F_OutputTime = 10f;
    public Light CookingLight = null;
    public Conveyor InputConveyor = null;
    public Conveyor OutputConveyor = null;
    public ItemDetectZone CookingTrigger = null;
    public ItemDetectZone CookingArea = null;

    private bool B_Init = true;
    private bool B_Cooking = false;

    private void OnEnable()
    {
        Init();
        
    }

    private void Init()
    {
        if (CookingLight == null) { Debug.LogError("CookerManager is missing a CookingLight"); B_Init = false; }
        if (InputConveyor == null) { Debug.LogError("CookerManager is missing an InputConveyor"); B_Init = false; }
        if (OutputConveyor == null) { Debug.LogError("CookerManager is missing an OutputConveyor"); B_Init = false; }
        if (CookingTrigger == null) { Debug.LogError("CookerManager is missing an CookingTrigger"); B_Init = false; }
        if (CookingArea == null) { Debug.LogError("CookerManager is missing an CookingArea"); B_Init = false; }

        if (B_Init)
        {
            CookingLight.enabled = false;
            StartInput();
            StartCoroutine(HandleInputs());
        }
    }

    private void StartInput()
    {        
        ToggleConveyor(1, false);
        ToggleConveyor(0, true);
        B_Cooking = false;
    }

    private IEnumerator HandleInputs()
    {
        while (true)
        {
            if (!B_Cooking)
            {
                // Check if there are items to cook.
                List<GameObject> DetectedObjects = CookingTrigger.GetDetectedObjects();
                if (DetectedObjects != null && DetectedObjects.Count > 0) // Time to Cook!
                {
                    B_Cooking = true;
                    ToggleConveyor(0, false);
                    CookingLight.enabled = true;
                    StartCoroutine(CookItems());
                }
            }
            yield return null;
        }
    }

    private IEnumerator CookItems()
    {
        float F_StartTime = Time.time;
        float F_CurrentTime = F_StartTime;

        while( F_CurrentTime - F_StartTime < F_CookTime) // Simulate Cooking Time.
        {
            F_CurrentTime = Time.time;
            yield return null;
        }

        List<GameObject> CookingObjects = CookingArea.GetDetectedObjects();
        foreach (GameObject CookingObject in CookingObjects)
        {
            CookableItem CookScript = CookingObject.GetComponent<CookableItem>();
            if (CookScript != null) CookScript.Cook();
        }
        StartCoroutine(HandleOutputs());        
    }

    private IEnumerator HandleOutputs()
    {
        float F_StartTime = Time.time;
        float F_CurrentTime = F_StartTime;
        CookingLight.enabled = false;
        ToggleConveyor(1, true);

        // Allow for cooked items to exit the cooker before switching on inputs again.
        while (F_CurrentTime - F_StartTime < F_OutputTime)
        {
            F_CurrentTime = Time.time; 
            yield return null;
        }

        StartInput();
    }

    private void ToggleConveyor(int i_ID, bool b_State)
    {
        Conveyor SelectedConveyor = null;
        if (i_ID == 0) SelectedConveyor = InputConveyor;
        else if (i_ID == 1) SelectedConveyor = OutputConveyor;

        if (SelectedConveyor != null)
        {
            Debug.Log("Selected a Conveyor!");
            if (b_State) SelectedConveyor.TurnOn();
            else SelectedConveyor.TurnOff();
        }
    }
}