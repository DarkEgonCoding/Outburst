using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameController.instance.ResumeGame();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GameController.instance.StartNewGame();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            GameController.instance.OpenLeaderboard();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // stops play mode in editor
        #else
            Application.Quit(); // quits the build
        #endif
        }
    }
}
