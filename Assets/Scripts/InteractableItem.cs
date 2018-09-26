using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    private GameObject GO_ControllerHeldItem_Left = null;
    private GameObject GO_ControllerHeldItem_Right = null;

    private void OnEnable()
    {
        BasicController.OnInteraction += HandleInteraction; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactor_Controller-Left")
        {
            Debug.Log(name + " Update - LeftControllerPresent");
            GO_ControllerHeldItem_Left = other.transform.parent.Find("HeldItem").gameObject;
        }
        else if (other.tag == "Interactor_Controller-Right")  // Left controller ALWAYS takes precedence for interaction handling
        {
            Debug.Log(name + " Update - RightControllerPresent");
            GO_ControllerHeldItem_Right = other.transform.parent.Find("HeldItem").gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactor_Controller-Left")
        {
            Debug.Log(name + " Update - LeftControllerAway");
            GO_ControllerHeldItem_Left = null;
        }
        else if (other.tag == "Interactor_Controller-Right")  // Left controller ALWAYS takes precedence for interaction handling
        {
            Debug.Log(name + " Update - RightControllerAway");
            GO_ControllerHeldItem_Right = null;
        }
    }

    // -- Custom Methods -- \\
    private void HandleInteraction(int i_ID, int i_State)
    {
        switch (i_ID)
        {
            case 0: // Left Controller Interactions
                if (i_State == 1 && GO_ControllerHeldItem_Left) // Start being held
                {
                    TogglePhysics(false);
                    transform.position = GO_ControllerHeldItem_Left.transform.position;
                    transform.parent = GO_ControllerHeldItem_Left.transform;                    
                }
                else if (transform.parent != null && transform.parent.name == "HeldItem")
                    TogglePhysics(true); // Let Physics take over if left was holding it
                break;

            case 1: // Right Controller Interactions
                if (i_State == 1 && GO_ControllerHeldItem_Right) // Start being held
                {
                    TogglePhysics(false);
                    transform.position = GO_ControllerHeldItem_Right.transform.position;
                    transform.parent = GO_ControllerHeldItem_Right.transform;
                }
                else if (transform.parent != null && transform.parent.name == "HeldItem")
                    TogglePhysics(true); // Let Physics take over if right was holding it
                break;

            default:
                break;
        }
    }

    private void TogglePhysics(bool b_State)
    {
        GetComponent<Rigidbody>().isKinematic = !b_State;
        GetComponent<Rigidbody>().useGravity = b_State;

        if (b_State)
        {
            GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            transform.parent = null;
        }
        else
            GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
    }
}