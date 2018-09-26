using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Controllables.ArtificialBased;

public class PizzaBox : MonoBehaviour {

    public VRTK_InteractableObject GrabableBox = null;
    public bool B_Grabbed = false;
    public bool B_Open = false;
    public bool B_PizzaStored = false;
    public float F_StackDistance = 0.1f;

    public Animator Anim_Box = null;

    public Rigidbody RB_Box = null;
    public Transform BoxBottom = null;
    public GameObject OpenColliders;
    public GameObject ClosedColliders;
    public TaggedSnapZone BoxStorage = null;
    public PizzaData StoredPizza;

    public bool B_Active = true;
    private bool B_Stacked = false;
    public Pizza MyPizza;

    private void OnEnable()
    {
        StartCoroutine(StackBox());
        StartCoroutine(TrackStoredData());
        GrabableBox.InteractableObjectUsed += UseBox;
        GrabableBox.InteractableObjectGrabbed += Grab;
        GrabableBox.InteractableObjectUngrabbed += UnGrab;
        if (OpenColliders != null && ClosedColliders != null)
        {
            OpenColliders.SetActive(false);
            ClosedColliders.SetActive(true);
        }
    }

    private void UseBox(object sender, InteractableObjectEventArgs e)
    {
        B_Open = !B_Open;
        if (Anim_Box != null) Anim_Box.SetBool("B_Open", B_Open);
        if (OpenColliders != null && ClosedColliders != null)
        {
            if (B_Open)
            {
                OpenColliders.SetActive(true);
                ClosedColliders.SetActive(false);
            }
            else
            {
                OpenColliders.SetActive(false);
                ClosedColliders.SetActive(true);
            }
        }
        //transform.parent.parent = null;
    }

    private void Grab(object sender, InteractableObjectEventArgs e)
    {
        B_Grabbed = true;
    }

    private void UnGrab(object sender, InteractableObjectEventArgs e)
    {
        B_Grabbed = false;
    }

    private IEnumerator StackBox()
    {
        Debug.DrawRay(BoxBottom.position, -BoxBottom.up * F_StackDistance, Color.magenta);
        while (B_Active)
        {
            RaycastHit hit;
            if (Physics.Raycast(BoxBottom.position, -BoxBottom.up, out hit, F_StackDistance) && hit.collider.tag == "Box" && !B_Grabbed)
            {
                if (!B_Stacked)
                {
                    B_Stacked = true;
                    RB_Box.isKinematic = true;
                    int iCount = 0;
                    Transform temp = hit.transform;
                    while (temp.parent != null && temp.parent.tag == temp.tag && iCount++ < 3) temp = temp.parent;
                    PizzaBox StackedBox = temp.GetComponent<PizzaBox>();
                    if (StackedBox.B_Grabbed) RB_Box.isKinematic = false;
                    else
                    {
                        transform.parent = temp;
                        //transform.rotation = Quaternion.identity;
                    }
                }
            }
            else
            {
                B_Stacked = false;
                RB_Box.isKinematic = false;
                transform.parent = null;
            }

            yield return null;
        }
    }

    private IEnumerator TrackStoredData()
    {
        while (B_Active)
        {
            if (BoxStorage.LI_InZoneObjects.Count > 0 || B_PizzaStored)
            {
                if (!B_PizzaStored)
                {
                    MyPizza = BoxStorage.LI_InZoneObjects[0].GetComponent<Pizza>();
                    if (MyPizza != null)
                    {
                        StoredPizza = MyPizza.Data;
                        MyPizza.RB_Pizza.isKinematic = true;
                        MyPizza.RB_Pizza.interpolation = RigidbodyInterpolation.None;
                        MyPizza.RB_Pizza.detectCollisions = false;
                        B_PizzaStored = true;
                    }
                }
            }
            else
            {
                StoredPizza = new PizzaData();
                B_PizzaStored = false;
            }
            yield return null;
        }
    }

    // TODO - USE THIS VERSION IN FUTURE!

    //public TransformFollower Lid = null;
    //public VRTK_InteractableObject GrabableBox = null;
    //public VRTK_ArtificialRotator LidRotator = null;

    //private void OnEnable()
    //{
    //    GrabableBox.InteractableObjectGrabbed += Grab;
    //    GrabableBox.InteractableObjectUngrabbed += UnGrab;
    //}

    //private void OnDisable()
    //{

    //}

    //private void Grab(object sender, InteractableObjectEventArgs e)
    //{
    //    if (Lid != null) Lid.ToggleActive(true);
    //    if (LidRotator != null) LidRotator.enabled = false;
    //    //transform.parent.parent = null;
    //}

    //private void UnGrab(object sender, InteractableObjectEventArgs e)
    //{
    //    if (Lid != null) Lid.ToggleActive(false);
    //    if (LidRotator != null) LidRotator.enabled = true;
    //    //transform.parent.parent = null;
    //}
}