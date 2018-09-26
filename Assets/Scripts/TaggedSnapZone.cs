using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggedSnapZone : MonoBehaviour {

    public Transform[] SnapPoints = null;
    public string[] SnapableTags = null;

    public List<GameObject> LI_InZoneObjects = null;

    private void OnEnable()
    {
        if (SnapPoints != null && SnapableTags != null) Init();
    }

    private void Init()
    {
        LI_InZoneObjects = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine("ProcessCollider", other);
    }

    private void OnTriggerExit(Collider other)
    {
        int iCount = 0;
        Transform Tr_Other = other.transform;
        while (Tr_Other.parent != null && Tr_Other.parent.tag == Tr_Other.tag && iCount++ < 2) Tr_Other = Tr_Other.parent;

        for (int i = 0; i < LI_InZoneObjects.Count; i++)
        {
            if (Tr_Other.gameObject == LI_InZoneObjects[i])
            {
                LI_InZoneObjects.RemoveAt(i);
                break;
            }
        }
    }

    private IEnumerator ProcessCollider(Collider other)
    {
        if (LI_InZoneObjects.Count >= SnapPoints.Length) yield break; // Zone Full.

        bool b_Valid = false;
        // Check if the collider has a valid 'Snapable' tag.
        for (int i = 0; i < SnapableTags.Length; i++) { if (SnapableTags[i] != null && SnapableTags[i] == other.tag) { b_Valid = true; break; } }
        if (b_Valid)
        {
            // Find the root tagged object.
            int iCount = 0;
            Transform Tr_Other = other.transform;
            while (Tr_Other.parent != null && Tr_Other.parent.tag == Tr_Other.tag && iCount++ < 2) Tr_Other = Tr_Other.parent;

            if (LI_InZoneObjects.Contains(Tr_Other.gameObject)) yield break; // Early exit for already snapped objects.            

            Debug.Log("Reached 1");
            LI_InZoneObjects.Add(Tr_Other.gameObject);
            //Rigidbody rb_Temp = Tr_Other.gameObject.GetComponent<Rigidbody>();
            //if (rb_Temp)
            //{
            //    rb_Temp.isKinematic = true;
            //    rb_Temp.interpolation = RigidbodyInterpolation.None;
            //    rb_Temp.collisionDetectionMode = CollisionDetectionMode.Discrete;
            //    rb_Temp.velocity.Set(0f, 0f, 0f);
            //}

            for (int i = 0; i < LI_InZoneObjects.Count; i++)
            {
                if (LI_InZoneObjects[i] == Tr_Other.gameObject && i < SnapPoints.Length)
                {
                    Rigidbody rb_temp = Tr_Other.GetComponentInChildren<Rigidbody>();
                    bool b_temp = false;
                    if (rb_temp != null)
                    {
                        b_temp = rb_temp.isKinematic;
                        rb_temp.isKinematic = true;
                    }
                    // Tr_Other.parent = null;
                    Tr_Other.parent = SnapPoints[i];
                    Tr_Other.position = SnapPoints[i].position;
                    Tr_Other.rotation = new Quaternion(0f, 0f, 0f, 0f);

                    if (rb_temp != null) rb_temp.isKinematic = b_temp;
                }
            }
        }
        yield return null;
    }

}