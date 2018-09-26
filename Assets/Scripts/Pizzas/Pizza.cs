using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Pizza : MonoBehaviour {
    
    public Collider PhysicsCollider = null;
    public Collider PaintingCollider = null;
    public PizzaData Data;
    public Rigidbody RB_Pizza = null;
    public GameObject GO_Model = null;
    public Sprite[] CrustTextures = null;
    public Sprite[] ToppingTextures = null;

    private Material MaterialPizzaCrust = null;
    private Material MaterialPizzaTopping = null;
    private VRTK_InteractableObject InteractableObject = null;

    private void OnEnable()
    {
        Init();
        CookableItem.CookedItem += HandleCookRequest;
    }


    private void OnDisable()
    {
        InteractableObject.InteractableObjectGrabbed -= Grab;
        InteractableObject.InteractableObjectUngrabbed -= UnGrab;
        CookableItem.CookedItem -= HandleCookRequest;
    }

    private void Init()
    {
        // Init Pizza Data
        Data = new PizzaData
        {
            state = PizzaData.ECookedState.Raw,
            Ingredients = new Dictionary<PizzaData.EIngredients, float>()            
        };
        Data.Ingredients.Add(PizzaData.EIngredients.TomatoSauce, 0f);
        Data.Ingredients.Add(PizzaData.EIngredients.Cheese, 0f);

        InteractableObject = GetComponent<VRTK_InteractableObject>();
        RB_Pizza = GetComponent<Rigidbody>();

        // Search for a deafault physics collider.
        if (PhysicsCollider == null) PhysicsCollider = GetComponent<Collider>();
        if (PaintingCollider != null) PaintingCollider.enabled = false;

        if (GO_Model != null)
        {
            Renderer renderer = GO_Model.GetComponent<Renderer>();
            MaterialPizzaCrust = renderer.materials[0];
            MaterialPizzaTopping = renderer.materials[1];
        }

        InteractableObject.InteractableObjectGrabbed += Grab;
        InteractableObject.InteractableObjectUngrabbed += UnGrab;        
    }

    private void Grab(object sender, InteractableObjectEventArgs e)
    {
        if (RB_Pizza)
        {
            RB_Pizza.isKinematic = true;
            RB_Pizza.interpolation = RigidbodyInterpolation.None;
        }
        transform.parent.parent = null;
    }

    private void UnGrab(object sender, InteractableObjectEventArgs e)
    {
        if (RB_Pizza)
        {
            RB_Pizza.isKinematic = false;
            RB_Pizza.interpolation = RigidbodyInterpolation.Interpolate;
        }
        transform.parent.parent = null;
    }   

    public void TogglePainting(bool b_State)
    {
        if (PaintingCollider == null) return;
        if (b_State)
        {
            RB_Pizza.isKinematic = true;
            PaintingCollider.enabled = true;
            PhysicsCollider.enabled = false;
        }
        else
        {
            PaintingCollider.enabled = false;
            PhysicsCollider.enabled = true;
            RB_Pizza.isKinematic = false;
        }
    }

    public void AddIngredient(PizzaData.EIngredients Ingredient, float f_Amount)
    {
        float f_Value = 0f;
        Data.Ingredients.TryGetValue(Ingredient, out f_Value);

        if (f_Value < 10f)
        {
            f_Value += f_Amount;
            if (f_Value > 10f) f_Value = 10f;

            Data.Ingredients.Remove(Ingredient);
            Data.Ingredients.Add(Ingredient, f_Value);
        }

        // Debug Only
        Data.Ingredients.TryGetValue(Ingredient, out f_Value);
        Debug.Log("New Ingredient Value:" + f_Value);
    }

    private void HandleCookRequest(GameObject go_sender)
    {
        if (go_sender == gameObject) Cook();
    }

    private void Cook()
    {
        Debug.Log("Pizza Cooked Successfully!!!");
        if (Data.state == PizzaData.ECookedState.Burnt) Invoke("PreAsh", 5f);
        else Data.state++;
        UpdateTex();
    }

    private void PreAsh()
    {
        transform.Translate(Vector3.up);
        Invoke("Ash", 0.1f);
    }

    private void Ash()
    {        
        Destroy(gameObject.transform.parent.gameObject);
    }
    

    private void UpdateTex()
    {
        if (Data.state == PizzaData.ECookedState.Cooked)
        {
            Debug.Log("Trying to set the pizza texture!!! (cooked)");
            // TODO - Add Topping Cook Effect here!!!
            GO_Model.GetComponent<Renderer>().materials[GetMaterialPos("Material_Pizza-Crust (Instance)")].SetTexture("_MainTex",CrustTextures[1].texture);
        }
        else if (Data.state == PizzaData.ECookedState.Burnt)
        {
            Debug.Log("Trying to set the pizza texture!!! (burnt)");
            // TODO - Add Topping Burn Effect here!!!
            GO_Model.GetComponent<Renderer>().materials[GetMaterialPos("Material_Pizza-Crust (Instance)")].SetTexture("_MainTex", CrustTextures[2].texture);
        }
    }

    private int GetMaterialPos(string s_Material)
    {
        Material[] mats = GO_Model.GetComponent<Renderer>().materials;
        int i_Pos = -1;
        while (i_Pos < mats.Length - 1)
        {
            if (mats[i_Pos + 1].name == s_Material) { Debug.Log("Found Material: " + s_Material + " | i_Pos = " + i_Pos); return i_Pos + 1; }
            i_Pos++;
        }

        return i_Pos;
    }
}

[Serializable]
public struct PizzaData
{
    public enum ECookedState { Raw, Cooked, Burnt }
    public enum EIngredients { TomatoSauce, Cheese }
    public ECookedState state;
    public Dictionary<EIngredients,float> Ingredients;
}