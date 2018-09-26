using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- Basic Controller V1.1 --- \\
//     ---------------------     \\
//  Provides basic 4 directional \\ 
// character control, mouse look \\
//    and OnInteraction events   \\
// ----------------------------- \\
// --- DANIEL J J GELDENHUYS --- \\
// ----------------------------- \\

public class BasicController : MonoBehaviour
{
    public delegate void PlayerAction(int I_ID, int I_State); // I_ID is the type of action. I_State is whether its an on or an off trigger
    public static event PlayerAction OnInteraction;

    [Range(0f,10f)]
    public float F_MoveSpeed = 1f;
    public KeyCode KC_LeftInteraction = KeyCode.Q;
    public KeyCode KC_RightInteraction = KeyCode.E;
    public Vector2 V2_MouseSensitivity = new Vector2(2.0f, 2f);
    public Vector2 V2_CameraRange_Vertical = new Vector2(-20.0f, 20.0f);
    public Rigidbody RB_Character = null;

    private float F_VerticalInput = 0f;
    private float F_HorizontalInput = 0f;
    private float F_MouseX = 0f;
    private float F_MouseY = 0f;

    private Vector2 V2_CameraRotation = new Vector2(0f, 0f);

    private void Update()
    {
        HandleInputs();
        MouseLook();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void HandleInputs()
    {
        if (OnInteraction != null)
        {
            if(Input.GetKey(KC_LeftInteraction))
                OnInteraction(0, 1);
            if (Input.GetKeyUp(KC_LeftInteraction))
                OnInteraction(0, 0);

            if(Input.GetKey(KC_RightInteraction))
                OnInteraction(1, 1);
            if (Input.GetKeyUp(KC_RightInteraction))
                OnInteraction(1, 0);
        }

        F_VerticalInput = Input.GetAxis("Vertical");
        F_HorizontalInput = Input.GetAxis("Horizontal");
        F_MouseX = Input.GetAxis("Mouse X");
        F_MouseY = Input.GetAxis("Mouse Y");
    }

    private void Move()
    {
        //gameObject.transform.Translate(F_MoveSpeed * F_HorizontalInput * 0.1f, 0f, F_MoveSpeed * F_VerticalInput * 0.1f);

        if (RB_Character && (Mathf.Abs(F_VerticalInput) > 0f) || Mathf.Abs(F_HorizontalInput) > 0f)
        {
            RB_Character.isKinematic = false;
            Vector3 v3_ForwardVelocity = transform.forward * F_VerticalInput * F_MoveSpeed;
            Vector3 v3_SideVelocity = transform.right * F_HorizontalInput * F_MoveSpeed;
            RB_Character.velocity = (v3_ForwardVelocity + v3_SideVelocity);
        }
        else
        {
            RB_Character.isKinematic = true;
            RB_Character.velocity = Vector3.zero;
        }
    }

    private void MouseLook()
    {
        V2_CameraRotation[0] += F_MouseX * V2_MouseSensitivity[0];
        transform.eulerAngles = new Vector3(0.0f, V2_CameraRotation[0], 0.0f);

        V2_CameraRotation[1] -= F_MouseY * V2_MouseSensitivity[1];

        if (V2_CameraRotation[1] < V2_CameraRange_Vertical[0])
        {
            V2_CameraRotation[1] += F_MouseY * V2_MouseSensitivity[1];
        }
        else if (V2_CameraRotation[1] > V2_CameraRange_Vertical[1])
        {
            V2_CameraRotation[1] += F_MouseY * V2_MouseSensitivity[1];
        }
        else
        {
            Transform TempCamera = transform.Find("Main Camera");
            TempCamera.eulerAngles = new Vector3(V2_CameraRotation[1], TempCamera.eulerAngles.y, 0.0f);
        }
    }
}