using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spreader : MonoBehaviour {

    public Transform[] SpreadableSides = null;
    public float F_SpreadingDistance = .03f;
    public float F_SpreadingRate = 0.0007f;
    public float F_MaxLiquidCapacity = 10f;
    public float F_CurrentLiquid = 0f;
    public Texture DefaultTexture = null;
    public Texture SauceTexture = null;
    public Material SpreaderMaterial = null;

    public Texture2D SauceSpreadTexture = null;
    public PaintBrush SpreadSide1 = null;
    public PaintBrush SpreadSide2 = null;

    private bool B_GettingSauce = false;
    private Pizza PaintingPizza = null;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        F_CurrentLiquid = 0f;
        UpdateTexture();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Bowl" && !B_GettingSauce)
        {
            B_GettingSauce = true; // Ensure a single process per 'dip' interaction.
            Transform Tr_Temp = collision.collider.transform;
            while (Tr_Temp.parent != null && Tr_Temp.parent.tag == Tr_Temp.tag) Tr_Temp = Tr_Temp.parent; // Find parent tag.

            Bowl ActiveBowl = Tr_Temp.gameObject.GetComponent<Bowl>();
            if (ActiveBowl == null) { Debug.LogError("Spreader Error: Interacting Bowl object missing a bowl script."); return; }

            F_CurrentLiquid += ActiveBowl.RemoveSauce(F_MaxLiquidCapacity - F_CurrentLiquid);
            UpdateTexture();
            StartCoroutine(SpreadSauce());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Bowl" && B_GettingSauce) B_GettingSauce = false;
    }

    private void UpdateTexture()
    {
        if (DefaultTexture != null && SauceTexture != null && SpreaderMaterial != null)
        {
            if (F_CurrentLiquid > 0f)
                SpreaderMaterial.SetTexture("_MainTex", SauceTexture);
            else
                SpreaderMaterial.SetTexture("_MainTex", DefaultTexture);
        }
    }

    private IEnumerator SpreadSauce()
    {
        TogglePainter(true);   
        while(F_CurrentLiquid > 0f)
        {
            bool b_PizzaAvailable = false;
            foreach (Transform t_CurrentSide in SpreadableSides)
            {
                RaycastHit hit1;
                if (Physics.Raycast(t_CurrentSide.position, t_CurrentSide.right, out hit1, F_SpreadingDistance))
                {
                    // TODO - Possibly put this within a check to see if we hit a Pizza to disable the rigidbody kinematic + convex collider while painting. Re-enable when not painting.
                    //Debug.Log("Raycast hit an object!");

                    if (hit1.collider.tag == "PizzaBase" || hit1.collider.tag == "PizzaTopping")
                    {
                        int iCount = 0;
                        Transform Tr_Other = hit1.transform;
                        while (Tr_Other.parent != null && Tr_Other.parent.tag == "PizzaBase" && iCount++ < 3) Tr_Other = Tr_Other.parent;
                        if (Tr_Other.parent != null) Tr_Other = Tr_Other.parent;

                        PaintingPizza = Tr_Other.Find("PizzaBase").GetComponent<Pizza>();
                        if (PaintingPizza != null)
                        {
                            PaintingPizza.TogglePainting(true);
                            PaintingPizza.AddIngredient(PizzaData.EIngredients.TomatoSauce, F_SpreadingRate);
                            F_CurrentLiquid -= F_SpreadingRate;
                            b_PizzaAvailable = true;
                        }
                    }                   
                }                
            }
            if (!b_PizzaAvailable) ResetPaintingPizza();
            //Debug.Log("Trying to spread sauce!");
            yield return new WaitForSecondsRealtime(0.03f);
        }
        ResetPaintingPizza();
        TogglePainter(false);
        UpdateTexture();
    }

    private void ResetPaintingPizza()
    {
        if (PaintingPizza != null)
        {
            PaintingPizza.TogglePainting(false);
            PaintingPizza = null;
        }
    }

    private void TogglePainter(bool b_State)
    {
        if (SpreadSide1 != null) SpreadSide1.B_Active = b_State;
        if (SpreadSide2 != null) SpreadSide2.B_Active = b_State;
    }
}