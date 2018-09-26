using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueezerManager : MonoBehaviour {
    public const float F_SQUEEZE_TIME = 1f;
    public const byte BYTE_INTENDED_MAX_BATCH_ITEMS = 5;

    public GameObject[] LiquidOutlets = null;

    public delegate void SqueezerAction(GameObject sender, bool b_State);
    public static event SqueezerAction SqueezeState;

    public Animator Anim_LeftOutlet = null;
    public Animator Anim_RightOutlet = null;

    public ItemDetectZone InputZone = null;
    public Animator AnimSqueezer = null;
    public Material Mat_Screen = null;
    public GameObject GO_Button_StartSqueezing = null;

    private bool B_Active = false;
    private bool B_OutletsOn = false;
    private GameObject GO_InputZoneObject = null;
    private byte Byte_AvailableTomatoes = 0;
    private byte Byte_TotalTomatoBatch = 0;
    private float F_CurrentDelay = 0f;
    private TouchButton TB_SqueezeButton = null;

    private void OnEnable()
    {
        StartCoroutine("Squeeze");
        TouchButton.ButtonTouch += HandleButtonTouch;
        Init();
    }

    private void Init()
    {
        if (GO_Button_StartSqueezing != null)
            TB_SqueezeButton = GO_Button_StartSqueezing.GetComponent<TouchButton>();
        if (Mat_Screen != null)
            Mat_Screen.SetFloat("_ScrollX_A", 0);
    }

    private void HandleButtonTouch(GameObject go_Sender)
    {
        if (go_Sender == GO_Button_StartSqueezing)
            if (B_Active == false) StartSqueezing();
    }

    public void StartSqueezing()
    {
        if (SqueezeState != null)
        {
            foreach (GameObject LiquidOutlet in LiquidOutlets)
            {
                SqueezeState(LiquidOutlet, true);
            }
        }

        UpdateAnimator(true);           
        Invoke("HandleInputItems", 1f); // Delayed to ensure tomatoes have all made it into the detection area after a rotator turn.
    }

    private void HandleInputItems()
    {
        if (InputZone != null)
        {
            List<GameObject> li_TempObjects = InputZone.GetDetectedObjects();
            List<GameObject> li_RemoveItems = new List<GameObject>();
            foreach (GameObject go_DetectedItem in li_TempObjects)
            {
                Debug.Log("Detected Object #");
                if (go_DetectedItem.tag == "Tomato") // Tomato Input Processing
                {
                    li_RemoveItems.Add(go_DetectedItem);
                }
                else // Invalid Input Processing
                    Debug.LogError("Invalid Item detected in Squeezer - write code to handle this later.");
            }

            if (li_RemoveItems.Count <= 0) // Early exit when no tomatoes should be processed.
            {
                StopSqueezing();
                return;
            }

            Byte_AvailableTomatoes = (byte)li_RemoveItems.Count;
            Byte_TotalTomatoBatch = Byte_AvailableTomatoes; // Update tomato batch total to track tomato batch processing progress (for uv scaling).
            InputZone.DestroyItems(li_RemoveItems);
            B_Active = true;
            F_CurrentDelay = 0; // Ensure Immediate tomato squeezing.
        }
    }

    private void UpdateAnimator(bool b_State)
    {
        if (AnimSqueezer != null)
            AnimSqueezer.SetBool("B_Active", b_State);
    }    

    private IEnumerator Squeeze()
    {
        while (true)
        {
            if (F_CurrentDelay >= F_SQUEEZE_TIME)
            {
                F_CurrentDelay = 0f;
                if (B_Active)
                {
                    if (Byte_AvailableTomatoes > 0)
                    {
                        // Switch the Outlets on for the first tomato in the batch.
                        if (!B_OutletsOn) UpdateOutlets(true);

                        Debug.Log("Processed tomato " + (1 + Byte_TotalTomatoBatch - Byte_AvailableTomatoes) + " of " + Byte_TotalTomatoBatch + " in the current batch.");
                        Byte_AvailableTomatoes--;
                    }
                    else
                    {
                        UpdateOutlets(false);
                        Byte_TotalTomatoBatch = 0;
                        StopSqueezing();
                    }
                }
            }
            else
            {
                F_CurrentDelay += Time.deltaTime;
                
                // Scale Screen indicator according to processing time.
                if (B_Active && Mat_Screen != null)
                {
                    float f_Scale = (float)BYTE_INTENDED_MAX_BATCH_ITEMS - (float)Byte_AvailableTomatoes;
                    if (f_Scale < 0) f_Scale = 0;
                    
                    float f_CurrentProgress = (f_Scale + (F_CurrentDelay / F_SQUEEZE_TIME));

                    //if (f_CurrentProgress > BYTE_INTENDED_MAX_BATCH_ITEMS) f_CurrentProgress = BYTE_INTENDED_MAX_BATCH_ITEMS;
                    Mat_Screen.SetFloat("_ScrollX_A", 1f-(f_CurrentProgress/(float)BYTE_INTENDED_MAX_BATCH_ITEMS));
                    
                    // Switch the Outlets off immediately after processing tomatoes.
                    //if (1f - (f_CurrentProgress / (float)BYTE_INTENDED_MAX_BATCH_ITEMS) <= 0f) UpdateOutlets(true);
                }
            }
            yield return null;
        }
    }

    private void StopSqueezing()
    {
        if (SqueezeState != null)
        {
            foreach (GameObject LiquidOutlet in LiquidOutlets)
            {
                SqueezeState(LiquidOutlet, false);
            }
        }
        B_Active = false;
        UpdateAnimator(false);
        TB_SqueezeButton.ResetButton();
    }

    private void UpdateOutlets(bool b_State)
    {
        B_OutletsOn = b_State;

        foreach (GameObject LiquidOutlet in LiquidOutlets)
        {
            Animator OutletAnimator = LiquidOutlet.gameObject.GetComponent<Animator>();
            if (OutletAnimator != null) OutletAnimator.SetBool("B_Active", b_State);
        }
        //if (Anim_LeftOutlet != null)
        //    Anim_LeftOutlet.SetBool("B_Active", b_State);
        //if (Anim_RightOutlet != null)
        //    Anim_RightOutlet.SetBool("B_Active", b_State);
    }
}