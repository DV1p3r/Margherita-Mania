using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public float F_Score = 0f;
    public Text Txt_ScoreDisplay = null;

    private bool B_Paused = false;

    private void OnEnable()
    {
        MenuManager.GameState += HandleGameStateChange;
        PizzaRater.AddScore += AddScore;
    }

    private void HandleGameStateChange(int iValue)
    {
        switch (iValue)
        {
            // Playing
            case 1:
                if (B_Paused) B_Paused = false;
                else { F_Score = 0f; Txt_ScoreDisplay.text = (Mathf.Round(F_Score)).ToString(); }
                break;
            // Paused
            case 2:
                B_Paused = true;
                break;
            // End-Game
            case 3:
                Debug.Log("######### ------------ FINAL SCORE = " + F_Score + " ------------ #########");
                break;
            default:
                break;
        }
    }

    private void AddScore(float f_Score)
    {
        F_Score += f_Score;
        Txt_ScoreDisplay.text = F_Score.ToString();
    }
}