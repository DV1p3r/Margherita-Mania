using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BoxGraber : MonoBehaviour {

    public GameObject GO_InZoneBox = null;
    public bool B_GrabbedBox = false;

    private void OnTriggerEnter(Collider other)
    {
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null /*&& Tr_Other.parent.tag == Tr_Other.tag*/ && iCount++ < 4) Tr_Other = Tr_Other.parent;
        if (Tr_Other.tag == "Box")
        {
            GO_InZoneBox = Tr_Other.gameObject;
            GO_InZoneBox.GetComponentInChildren<VRTK_InteractableObject>().InteractableObjectUngrabbed += GrabBox;
        }
    }

    private void GrabBox(object sender, InteractableObjectEventArgs e)
    {
        if (GO_InZoneBox == null) return;
        Debug.Log("TryingToGrabTheBox!");
        GO_InZoneBox.GetComponent<PizzaBox>().B_Active = false;
        Rigidbody[] rigidbodies = GO_InZoneBox.GetComponentsInChildren<Rigidbody>();
        for (int iCount = 0; iCount < rigidbodies.Length; iCount++)
        {
            Rigidbody body = rigidbodies[iCount];
            Debug.Log("Disabling RB: " + body.name);
            StartCoroutine(ToggleRB());
            body.detectCollisions = false;
        }
        if (GO_InZoneBox.GetComponent<Rigidbody>() != null) GO_InZoneBox.GetComponent<Rigidbody>().isKinematic = true;

        GO_InZoneBox.transform.parent = transform;
        GO_InZoneBox.transform.position = transform.position;

        B_GrabbedBox = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (B_GrabbedBox) return;
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null && Tr_Other.parent.tag == Tr_Other.tag && iCount++ < 2) Tr_Other = Tr_Other.parent;
        if (Tr_Other.tag == "Box" && GO_InZoneBox != null)
        {
            GO_InZoneBox.GetComponentInChildren<VRTK_InteractableObject>().InteractableObjectUngrabbed -= GrabBox;
            GO_InZoneBox = null;
        }
    }

    private IEnumerator ToggleRB()
    {
        Rigidbody body = GO_InZoneBox.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.3f);
        while (body.isKinematic == false)
        {
            body.isKinematic = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
