using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookableItem : MonoBehaviour {

    public delegate void CookAction(GameObject sender);
    public static event CookAction CookedItem;

    public void Cook()
    {
        Debug.Log("Cooked Item: " + gameObject.name);
        if (CookedItem != null) CookedItem(gameObject);
    }
}