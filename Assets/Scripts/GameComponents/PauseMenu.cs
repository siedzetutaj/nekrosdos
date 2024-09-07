using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourSingleton<PauseMenu>
{
    public static bool gameIsPaused = false;
    public static GameObject pauseMenuUI;
    public static Action unPaused;

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
        SwitchPause();
        LoadDataHelper.Instance.LoadSceneFromSaveData();
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
