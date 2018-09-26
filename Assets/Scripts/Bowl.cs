using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour {

    public float F_LiquidFillRate = 0.07f;
    public float F_MaxLiquidCapacity = 100f;

    public float F_CurrentLiquid = 0f;
    private bool B_Filling = false;

    public GameObject ActiveSqueezer = null;
    private SkinnedMeshRenderer ThisRenderer = null;

    public float RemoveSauce(float f_ContainerCapacity)
    {
        float f_Temp = 0f;
        if (F_CurrentLiquid <= 0f) return f_Temp; // Early Exit for Empty bowls.

        if (F_CurrentLiquid >= f_ContainerCapacity) // Possible to fill the interacting container fully.
        {
            F_CurrentLiquid -= f_ContainerCapacity; 
            f_Temp = f_ContainerCapacity;
        }
        else // Possible to fill the interacting container only partly.
        {
            f_Temp = F_CurrentLiquid;
            F_CurrentLiquid = 0f;
        }

        if (f_Temp > 0) ThisRenderer.SetBlendShapeWeight(0, 100f * (F_CurrentLiquid / F_MaxLiquidCapacity));

        return f_Temp;
    }

    private void OnEnable()
    {
        ThisRenderer = transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
        SqueezerManager.SqueezeState += HandleSqueezerUpdates;   
    }

    private void OnDisable()
    {
        SqueezerManager.SqueezeState -= HandleSqueezerUpdates;
    }

    private void HandleSqueezerUpdates(GameObject OriginSqueezer, bool State)
    {
        if (ThisRenderer != null && ActiveSqueezer != null)
        {
            Debug.Log("Check1");
            if (ActiveSqueezer == OriginSqueezer)
            {
                Debug.Log("Check2");
                if (State) Invoke("StartFilling", 2.77f);
                else Invoke("StopFilling", 0.7f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SqueezerOutlet")
            ActiveSqueezer = other.transform.parent.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SqueezerOutlet")
            ActiveSqueezer = null;
    }

    private void StartFilling()
    {
        B_Filling = true;
        StartCoroutine(FillBowl());
    }

    private void StopFilling()
    {
        B_Filling = false;
    }

    private IEnumerator FillBowl()
    {
        float f_CurrentFillPoint = F_CurrentLiquid / F_MaxLiquidCapacity;
        while (B_Filling && f_CurrentFillPoint < 1f)
        {
            ThisRenderer.SetBlendShapeWeight(0, f_CurrentFillPoint * 100f);
            F_CurrentLiquid += F_LiquidFillRate;
            f_CurrentFillPoint = F_CurrentLiquid / F_MaxLiquidCapacity;
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }
}