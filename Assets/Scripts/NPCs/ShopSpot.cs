using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpot : MonoBehaviour
{
    public int I_ID = -1;
    public GameObject GO_OrderPanel = null;
    public Sprite[] DisplayTextures = null;

    private Animator Anim_Panel = null;
    private bool B_Active = false;
    private bool B_OrderVisible = false;
    private GameObject GO_Screen = null;
    public Texture Tex_Screen = null;

    private void OnEnable()
    {
        if (GO_OrderPanel != null)
        {
            Anim_Panel = GO_OrderPanel.GetComponent<Animator>();
            GO_Screen = GO_OrderPanel.transform.GetChild(1).gameObject;
        }
    }

    public void ToggleActive(bool b_state)
    {
        B_Active = b_state;
        if (GO_OrderPanel != null)
        {
            if (B_Active) GO_OrderPanel.SetActive(true);
            else GO_OrderPanel.SetActive(false);
        }
        Debug.Log(gameObject.name + " is now " + (B_Active ? "Active" : "Disabled"));
    }

    public void ToggleOrder(PizzaOrder order)
    {
        B_OrderVisible = !B_OrderVisible;
        if (B_OrderVisible)
        {
            float f_delay = Random.Range(1f, 3f);
            Tex_Screen = GetOrderTex(order);
            Invoke("DisplayOrder", f_delay);
        }
        else
        {
            Tex_Screen = DisplayTextures[0].texture;
            if (GO_Screen != null) GO_Screen.GetComponent<Renderer>().material.SetTexture("_MainTex", Tex_Screen);
            Anim_Panel.SetBool("B_OrderPresent", false);
        }
    }

    private void DisplayOrder()
    {
        if (GO_Screen != null)
        {
            Debug.Log("Attempting to change screen texture!");
            GO_Screen.GetComponent<Renderer>().materials[GetMaterialPos("Material_Screen-Display (Instance)")].SetTexture("_MainTex", Tex_Screen);
        }
        Anim_Panel.SetBool("B_OrderPresent", true);
    }

    private Texture GetOrderTex(PizzaOrder order)
    {
        if (DisplayTextures == null) return null;

        bool b_Cheese = false;
        bool b_Tomato = false;
        order.Ingredients.TryGetValue(PizzaData.EIngredients.Cheese, out b_Cheese);
        order.Ingredients.TryGetValue(PizzaData.EIngredients.TomatoSauce, out b_Tomato);

        if (b_Cheese && b_Tomato) { Debug.Log("~~ Margherita Pizza!"); return DisplayTextures[3].texture; }
        else if (b_Cheese) { Debug.Log("~~ Cheese Pizza!"); return DisplayTextures[2].texture; }
        else if (b_Tomato) { Debug.Log("~~ Tomato Pizza!"); return DisplayTextures[1].texture; }
        else return DisplayTextures[0].texture;

    }

    private int GetMaterialPos(string s_Material)
    {
        Material[] mats = GO_Screen.GetComponent<Renderer>().materials;
        int i_Pos = -1;
        while (i_Pos < mats.Length - 1)
        {
            if (mats[i_Pos + 1].name == s_Material) { Debug.Log("Found Material: " + s_Material + " | i_Pos = " + i_Pos); return i_Pos + 1; }
            i_Pos++;
        }

        return i_Pos;
    }
}