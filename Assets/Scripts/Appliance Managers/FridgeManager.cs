using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeManager : MonoBehaviour {

    public GameObject GO_Cheese = null;
    public GameObject GO_CheeseDoor = null;
    public GameObject GO_Tomato = null;
    public GameObject GO_TomatoDoor = null;
    public GameObject GO_Button_Tomatoes = null;
    public GameObject GO_Button_Cheese = null;

    public GridSpawner GS_Tomatoes = null;
    public GridSpawner GS_Cheese = null;

    public Vector3 V3_CheeseSpawnSize = new Vector3();
    public Vector3 V3_CheeseSpawnSpacing = new Vector3();
    public Vector3 V3_TomatoSpawnSize = new Vector3();
    public Vector3 V3_TomatoSpawnSpacing = new Vector3();

    public Sprite[] ScreenTextures = new Sprite[4];
    public Material ScreenMaterial = null;

    private Animator Anim_Fridge = null;
    private bool B_CheeseOpen = false;
    private bool B_TomatoesOpen = false;
    private Transform T_CheeseSpawnSpots = null;
    private Transform T_TomatoSpawnSpots = null;

    

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Anim_Fridge = GetComponent<Animator>();

        if (GO_CheeseDoor) T_CheeseSpawnSpots = GO_CheeseDoor.transform.Find("SpawnSpots");        
        if (GO_TomatoDoor) T_TomatoSpawnSpots = GO_TomatoDoor.transform.Find("SpawnSpots");

        TouchButton.ButtonTouch += HandleButtonTouch;
        MenuManager.GameState += HandleStateUpdates;
    }

    // Refill the respective drawer.
    public void Respawn(bool b_Cheese) 
    {
        if (b_Cheese == true && T_CheeseSpawnSpots != null) // Cheese.
        {
            GS_Cheese.MultiGridSpawn(T_CheeseSpawnSpots, GO_Cheese, V3_CheeseSpawnSize, V3_CheeseSpawnSpacing,GO_CheeseDoor.transform);
        }
        else if (b_Cheese == false && T_TomatoSpawnSpots != null)// Tomatoes.
        {
            GS_Tomatoes.MultiGridSpawn(T_TomatoSpawnSpots, GO_Tomato, V3_TomatoSpawnSize, V3_TomatoSpawnSpacing,GO_TomatoDoor.transform);
        }
    }

    // Open/Close the respective drawer.
    public void ToggleDrawer(bool b_Cheese)
    {
        if (b_Cheese == true) // Cheese.
        {
            B_CheeseOpen = !B_CheeseOpen;
            Anim_Fridge.SetBool("B_Open-Left", B_CheeseOpen);
        }
        else // Tomatoes.
        {
            B_TomatoesOpen = !B_TomatoesOpen;
            Anim_Fridge.SetBool("B_Open-Right", B_TomatoesOpen);
        }
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        if (ScreenMaterial != null)
        {
            try
            {
                if (B_CheeseOpen)
                {
                    if (B_TomatoesOpen)
                        ScreenMaterial.SetTexture("_MainTex", ScreenTextures[3].texture);
                    else
                        ScreenMaterial.SetTexture("_MainTex", ScreenTextures[2].texture);
                }
                else
                {
                    if (B_TomatoesOpen)
                        ScreenMaterial.SetTexture("_MainTex", ScreenTextures[1].texture);
                    else
                        ScreenMaterial.SetTexture("_MainTex", ScreenTextures[0].texture);
                }
            }
            catch
            {
                Debug.LogError("FridgeManager can't find a required Screen Texture.");
            }
        }
    }

    private void HandleButtonTouch(GameObject go_Sender)
    {
        if (go_Sender == GO_Button_Cheese) ToggleDrawer(true);
        if (go_Sender == GO_Button_Tomatoes) ToggleDrawer(false);
    }

    private void HandleStateUpdates(int iValue)
    {
        if (iValue == (int)MenuManager.EGameState.Playing)
        {
            Respawn(true); // Spawn Cheese.
            Respawn(false); // Spawn Tomatoes.
        }
    }
}