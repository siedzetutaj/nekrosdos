using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviourSingleton<PauseMenu>
{
    public static bool gameIsPaused = false;
    public static GameObject pauseMenuUI;
    public static Action unPaused;

    private PlayerController _playerController;

    private void Start()
    {
        _playerController = PlayerController.Instance;
    }
    public void ResumeButton()
    {
        SwitchPause();
    }
    public static void SwitchPause()
    {
        pauseMenuUI.SetActive(gameIsPaused);
        if (gameIsPaused)
            Time.timeScale = 0;
        else
        {
            Time.timeScale = 1;
            unPaused?.Invoke();
        }

        gameIsPaused = !gameIsPaused;
    }
    public void SaveButton()
    {
        SaveSystem.SaveData();
    }
    public void LoadButton()
    {
        SaveData data = SaveSystem.LoadData();
        Vector3 pos = new Vector3(data._playerPosition[0], data._playerPosition[1], data._playerPosition[2]);
        _playerController.gameObject.transform.position = pos;

        UIInformationDisplay.Instance.LoadThoughts(data.unlockedThoughtsSO);
    
    }
    public void SettingsButton()
    {
        Debug.Log("To be implemented");
    }
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
