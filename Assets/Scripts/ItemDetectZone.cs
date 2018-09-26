using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ItemDetectZone : MonoBehaviour {

    private List<GameObject> LI_InZoneObjects = null;

    private void Start()
    {
        LI_InZoneObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ProcessCollider(other));
    }

    private void OnTriggerExit(Collider other)
    {
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null && Tr_Other.parent.tag == Tr_Other.tag && iCount++ < 2) Tr_Other = Tr_Other.parent;

        if (Tr_Other.gameObject.GetComponent<VRTK_InteractableObject>() != null)
            LI_InZoneObjects.Remove(Tr_Other.gameObject);
    }

    public List<GameObject> GetDetectedObjects()
    {
        //CleanList(); // Ensure no null references get sent through to GetDetectedObject requests. Omitted from when the actual trigger processing happens to save on performance.
        return LI_InZoneObjects;
    }

    public void DestroyItem (GameObject go_Item)
    {
        if (go_Item != null)
        {
            LI_InZoneObjects.Remove(go_Item);
            Destroy(go_Item);
        }
    }

    public void DestroyItems(List<GameObject> li_Items)
    {
        foreach (GameObject go_Item in li_Items)
        {
            DestroyItem(go_Item);
        }
    }

    private IEnumerator ProcessCollider(Collider other)
    {
        // TODO: Find Root GO.
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null && Tr_Other.parent.tag == Tr_Other.tag && iCount++ < 2) Tr_Other = Tr_Other.parent;

        if (!LI_InZoneObjects.Contains(Tr_Other.gameObject))
            if (Tr_Other.gameObject.GetComponent<VRTK_InteractableObject>() != null) // Check Parent for cases where scaled mesh colliders are used.
                LI_InZoneObjects.Add(Tr_Other.gameObject);

        yield return null;
    }
}