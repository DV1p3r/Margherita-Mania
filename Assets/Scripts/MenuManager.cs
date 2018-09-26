using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public Material MenuMaterial = null;
    public Sprite[] MenuTextures = null;
    public GameObject GO_ButtonStart = null;
    public GameObject GO_ButtonNormal = null;
    public GameObject GO_ButtonExpert = null;
    public GameObject GO_ButtonManiac = null;

    public delegate void GameAction(int iValue);
    public static event GameAction GameState;
    public static event GameAction GameMode;
    
    public enum EGameState { Menu, Playing, Paused, End}
    private int I_State = -1;
    public enum EMode { Normal, Expert, Maniac}
    private int I_Mode = -1;

    private void OnEnable()
    {
        ClientManager.ClientsState += HandleClientUpdates;
        TouchButton.ButtonTouch += HandleButtonTouch;

        I_Mode = (int)EMode.Normal; 
        BroadcastMode();
        I_State = (int)EGameState.Menu;
        BroadcastGameState();

        UpdateMenu();
    }

    private void OnDisable()
    {
        TouchButton.ButtonTouch -= HandleButtonTouch;
    }

    private void UpdateMenu()
    {
        if (MenuMaterial != null)
        {
            try
            {
                switch (I_Mode)
                {
                    case (int)EMode.Normal:
                        MenuMaterial.SetTexture("_MainTex", MenuTextures[0].texture);
                        break;
                    case (int)EMode.Expert:
                        MenuMaterial.SetTexture("_MainTex", MenuTextures[1].texture);
                        break;
                    default:
                        break;                    
                }                
            }
            catch
            {
                Debug.LogError("MenuManager can't find a required Screen Texture.");
            }
        }
    }

    private void HandleButtonTouch(GameObject go_Sender)
    {
        if (go_Sender == GO_ButtonStart && I_State == (int)EGameState.Menu)
        {
            Debug.Log("-------- GAME START --------");
            I_State = (int)EGameState.Playing;
            BroadcastMode();
            BroadcastGameState();
            return;
        }

        bool b_ModeChange = false;
        if (go_Sender == GO_ButtonNormal) { I_Mode = (int)EMode.Normal; b_ModeChange = true; }
        if (go_Sender == GO_ButtonExpert) { I_Mode = (int)EMode.Expert; b_ModeChange = true; }
        //if (go_Sender == GO_ButtonManiac) I_Mode = (int)EMode.Expert; -- TODO!!! Uncomment once Mode adjustments have been made. TODO!!!

        if (b_ModeChange)
        {
            UpdateMenu();
            BroadcastMode();
        }        
    }

    private void HandleClientUpdates(int iValue)
    {
        I_State = iValue;
        BroadcastGameState();
    }

    private void BroadcastGameState()
    {
        if (GameState != null) GameState(I_State);
    }

    private void BroadcastMode()
    {
        if (GameMode != null) GameMode(I_Mode);
    }
}