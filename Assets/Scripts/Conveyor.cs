using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {

    public ItemDetectZone ConveyorZone = null;
    public bool B_Active = false;
    public float F_Speed = 0.03f;
    public bool B_ScrollA = true;
    public Material ScrollingMaterial = null;

    private float F_ScrollPos = 1f;

    public void TurnOn()
    {
        B_Active = true;
    }

    public void TurnOff()
    {
        B_Active = false;
    }

    private void Start()
    {
        Init();  
    }

    private void Init()
    {
        if (ConveyorZone == null)
        {
            try { ConveyorZone = GetComponent<ItemDetectZone>(); } // Attempt to find the zone at runtime.
            catch { Debug.LogError("Conveyor is Missing and ItemDetectZone!"); return; }
        }
        StartCoroutine(MoveItems());
    }

    private IEnumerator MoveItems()
    {
        while (true)
        {
            if (B_Active)
            {
                // Move objects on the Conveyor
                Vector3 v3_MovingDirection = gameObject.transform.right * -1f * F_Speed;
                List<GameObject> MovingObjects = ConveyorZone.GetDetectedObjects();                                
                foreach (GameObject ActiveObject in MovingObjects)
                {
                    ActiveObject.transform.Translate(v3_MovingDirection,Space.World);
                }

                // "Animate" the Conveyor texture
                if (ScrollingMaterial != null)
                {
                    if (B_ScrollA)
                        ScrollingMaterial.SetFloat("_ScrollX_A", F_ScrollPos);
                    else
                        ScrollingMaterial.SetFloat("_ScrollX_B", (1-F_ScrollPos) < 0f? 0f : 1 - F_ScrollPos);
                    F_ScrollPos -= F_Speed;
                    if (F_ScrollPos <= 0f) F_ScrollPos += 1f;
                }
            }
            yield return null;
        }
    }
}