using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FreezerManager : MonoBehaviour
{
    public GameObject GO_Button_Pizza = null;

    public Sprite[] ScreenTextures = new Sprite[2];
    public Material ScreenMaterial = null;
    public GameObject[] PizzaHolders = null;

    public GameObject[] PizzaBases = new GameObject[2];

    private PizzaDetector[] PizzaDetectors = null;
    private Animator Anim_Freezer = null;
    private bool B_Open = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Anim_Freezer = GetComponent<Animator>();
        TouchButton.ButtonTouch += HandleButtonTouch;
        PizzaDetector.SlotOpen += HandlePizzaSpawn;
        UpdateScreen();

        if (PizzaBases[0] == null) { Debug.LogError("FreezerManager is missing a default PizzaBase type GameObject"); return; }

        if (PizzaHolders != null)
        {
            PizzaDetectors = new PizzaDetector[PizzaHolders.Length];
            int i = 0;
            foreach (GameObject PizzaHolder in PizzaHolders)
            {
                PizzaDetectors[i] = PizzaHolder.transform.Find("PizzaSlot").gameObject.GetComponent<PizzaDetector>();
                StartCoroutine(SpawnPizza(i,false));
                i++;
            }
        }
        else Debug.LogError("FreezerManager is missing a default PizzaHolder");
    }

    private void HandlePizzaSpawn(GameObject Pizza)
    {
        StartCoroutine(SpawnNewPizza(Pizza));
    }

    // Open/Close the respective drawer.
    public void ToggleDrawer()
    {
        B_Open = !B_Open;
        Anim_Freezer.SetBool("B_Open", B_Open);
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        if (ScreenMaterial != null)
        {
            try
            {
                if (!B_Open)
                    ScreenMaterial.SetTexture("_MainTex", ScreenTextures[0].texture);
                else
                    ScreenMaterial.SetTexture("_MainTex", ScreenTextures[1].texture);
            }
            catch
            {
                Debug.LogError("FreezerManager can't find a required Screen Texture.");
            }
        }
    }

    private void HandleButtonTouch(GameObject go_Sender)
    {
        if (go_Sender == GO_Button_Pizza) ToggleDrawer();
    }

    private IEnumerator SpawnNewPizza(GameObject PreviousPizza)
    {
        int iPos = -1;

        foreach (PizzaDetector CurrentSlot in PizzaDetectors)
        {
            if (CurrentSlot.GO_ActivePizza == PreviousPizza) { iPos++; break; }
            iPos++;
        }

        if (iPos <= -1) { Debug.LogError("FreezerManager couldn't find the grabbed pizza's slot."); yield break; }

        ToggleExtender(iPos, false);
        StartCoroutine(SpawnPizza(iPos, true));

        yield return null;
    }

    private IEnumerator SpawnPizza(int iPos, bool b_Delayed)
    {
        if (b_Delayed) yield return new WaitForSeconds(2f);
        if (PizzaDetectors[iPos].B_PizzaPresent) yield break; // Prevent Double Spawns.

        GameObject NewPizza = Instantiate(PizzaBases[0], PizzaHolders[iPos].transform);
        //Rigidbody RB_Temp = NewPizza.GetComponent<Rigidbody>();
        //if (RB_Temp != null) RB_Temp.interpolation = RigidbodyInterpolation.None;
        Vector3 NewPos = new Vector3(0f, 0.19f, 0f);
        NewPizza.transform.localPosition = NewPos;
        NewPizza.transform.rotation.eulerAngles.Set(0f, 0f, -90f);
        PizzaDetectors[iPos].B_PizzaPresent = true;
        PizzaDetectors[iPos].GO_ActivePizza = NewPizza;
        if (b_Delayed) ToggleExtender(iPos, true);

        yield return null;
    }

    private void ToggleExtender(int i_ID, bool b_State)
    {
        if (i_ID == 0) Anim_Freezer.SetBool("B_LeftHeld", b_State);
        else if (i_ID == 1) Anim_Freezer.SetBool("B_RightHeld", b_State);
    }
}