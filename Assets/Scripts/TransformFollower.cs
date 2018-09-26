using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour {

    public Transform toFollow;
    public bool B_AutoRotate = true;
    public Rigidbody RB_ToTrack = null;
    //private Quaternion rotationOffset = new Quaternion(); TODO.
    private Vector3 offset;
    public bool B_Active = true;

    void Start()
    {
        offset = toFollow.position - transform.position;
    }

    void Update()
    {
        if (RB_ToTrack != null && RB_ToTrack.velocity.magnitude > 0.000001f) B_Active = true;
        if (B_Active)
        {
            if (toFollow == null) Destroy(gameObject);
            transform.position = toFollow.position - offset;
            if (B_AutoRotate) transform.rotation = toFollow.rotation;
        }
    }

    public void ToggleActive(bool b_State)
    {
        if (!b_State && RB_ToTrack != null) // If stopping tracking and we and we want to keep with up physics until standstil.
        {
            StartCoroutine(DeactivateTracking());
            return;
        }

        B_Active = b_State;
        if (B_Active) offset = toFollow.position - transform.position; // Update offset after re-activation.
    }

    private IEnumerator DeactivateTracking()
    {
        float f_HardDelay = .3f;
        float f_CurrentDelay = 0f;   
        while (RB_ToTrack.velocity.magnitude > 0.000001f || f_CurrentDelay < f_HardDelay)
        {
            f_CurrentDelay += Time.deltaTime;
            Debug.Log("Waiting for 0 Velovity");
            yield return null;
        }

        B_Active = false;
    }
}