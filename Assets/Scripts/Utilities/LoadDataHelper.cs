using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDataHelper : MonoBehaviourSingletonPersistent<LoadDataHelper>
{
    private SaveData data = null;
    public bool isSwitchingScene = false;

    public  void LoadSceneFromSaveData()
    {
        data = SaveSystem.LoadData();

        SceneManager.LoadSceneAsync(data.currentSceneName);
    }
    public void LoadSaveData()
    {
        data = SaveSystem.LoadData();
        //Checks if Data is loaded from save or loaded due to scene transition
        UIInformationDisplay.Instance.LoadInformations(data.unlockedInformationsSO);
        UiThoughtPanel.Instance.LoadThoughts(data.thoughts);
        UiThoughtPanel.Instance.LoadConnections(data.lineConnectionGuids);
        if (!isSwitchingScene)
        {
            Vector3 pos = new Vector3(data._playerPosition[0], data._playerPosition[1], data._playerPosition[2]);
            PlayerController.Instance.gameObject.transform.position = pos;
        }
        else
        {
            var previousScene = (SceneNames)Enum.Parse(typeof(SceneNames), data.currentSceneName);
            BasicObjectsLoader.Instance.SetPlayerToSpawnPoint(previousScene);
            SaveSystem.SaveData();
        }
    }
}
