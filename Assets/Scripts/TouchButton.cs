using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class TouchButton : MonoBehaviour {

    public delegate void TouchScreenAction(GameObject Sender);
    public static event TouchScreenAction ButtonTouch;

    public bool B_3DButton = false, B_AutoReset = false;
    public float F_ResetDelay = 0f;
    public Color Colour_NotPressed, Colour_Pressed;

    public GameObject GO_Controller1 = null, GO_Controller2 = null;
    public GameObject GO_Button = null;

    private VRTK_InteractNearTouch VRTK_InteractNearTouch1, VRTK_InteractNearTouch2;
    private Animator Anim_Button = null;
    private Material Mat_Button = null;


    private void Init()
    {
        // Get the required Controller VRTK_InteractNearTouch scripts.
        VRTK_InteractNearTouch1 = GO_Controller1.GetComponent<VRTK_InteractNearTouch>();
        VRTK_InteractNearTouch2 = GO_Controller2.GetComponent<VRTK_InteractNearTouch>();

        // Get the required button Animator and Material if it's a 3D Touch Button.
        if (B_3DButton && GO_Button != null)
        {
            Anim_Button = GO_Button.GetComponent<Animator>();
            Mat_Button = GO_Button.GetComponent<Renderer>().material;
            Mat_Button.color = Colour_NotPressed;
        }
    }

    private void OnEnable()
    {
        Init();
        VRTK_InteractNearTouch1.ControllerNearTouchInteractableObject += Touch;
        VRTK_InteractNearTouch2.ControllerNearTouchInteractableObject += Touch;
    }

    private void OnDisable()
    { 
        VRTK_InteractNearTouch1.ControllerNearTouchInteractableObject -= Touch;
        VRTK_InteractNearTouch2.ControllerNearTouchInteractableObject -= Touch;
    }

    // VRTK_InteractNearTouch Event Handling
    private void Touch(object sender, ObjectInteractEventArgs e)
    {
        if (e.target.gameObject == this.gameObject) // Confirm this is the NearTouch target.
        {
            if (ButtonTouch != null) ButtonTouch(gameObject); // Notify Subscribers.

            Debug.Log("Triggering Touch!");
            // 3D Button Handling
            if (B_3DButton) 
            {
                Anim_Button.SetBool("B_Pressed",true);
                Mat_Button.color = Colour_Pressed;
                if (B_AutoReset) Invoke("ResetButton",F_ResetDelay);
            }
        }
    }

    // Public Animator Reset method for Non-AutoReset 3D Buttons.
    public void ResetButton()
    {
        if (B_3DButton)
        {
            Anim_Button.SetBool("B_Pressed", false);
            Mat_Button.color = Colour_NotPressed;        
        }
    }
}