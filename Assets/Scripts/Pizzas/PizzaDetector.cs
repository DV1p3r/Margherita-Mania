using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaDetector : MonoBehaviour {

    public delegate void FreezerDetector(GameObject Pizza);
    public static event FreezerDetector SlotOpen;

    public bool B_PizzaPresent = false;
    public string S_PizzaBaseTag = "PizzaBase";
    public GameObject GO_ActivePizza = null;

    private void OnTriggerEnter(Collider other)
    {
        if (B_PizzaPresent || other.gameObject.tag == null || other.gameObject.tag != S_PizzaBaseTag) return; // Early exit when full or for non-pizzabase/untagged collider gameobjects.

        B_PizzaPresent = true;

        // Find the root tagged object.
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null && Tr_Other.parent.gameObject.tag == Tr_Other.gameObject.tag && iCount++ < 3) Tr_Other = Tr_Other.parent;
        Tr_Other = Tr_Other.parent;

        GO_ActivePizza = Tr_Other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (GO_ActivePizza == null || other.tag == null) return;
        Transform temp = other.transform;
        while (temp.parent != null && temp.parent.tag == temp.tag) temp = temp.parent;
        if (temp.parent != null) temp = temp.parent;

        if (temp.gameObject == GO_ActivePizza)
        {
            B_PizzaPresent = false;

            GO_ActivePizza.transform.parent = null;

            Rigidbody RB_Temp = GO_ActivePizza.GetComponent<Rigidbody>();
            if (RB_Temp != null)
            {
                Debug.Log("found a rigidBody.");
                RB_Temp.isKinematic = false;
                RB_Temp.interpolation = RigidbodyInterpolation.Interpolate;
            }

            if (SlotOpen != null) SlotOpen(GO_ActivePizza);

            GO_ActivePizza = null;
        }
    } 
}