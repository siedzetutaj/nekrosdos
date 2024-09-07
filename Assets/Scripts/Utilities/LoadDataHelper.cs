using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDataHelper : MonoBehaviourSingletonPersistent<LoadDataHelper>
{
    private SaveData data = null;
    public void LoadWhenSwitchScene()
    {
        SaveData data = SaveSystem.LoadData();

        UIInformationDisplay.Instance.LoadInformations(data.unlockedInformationsSO);
        UiThoughtPanel.Instance.LoadThoughts(data.thoughts);
        UiThoughtPanel.Instance.LoadConnections(data.lineConnectionGuids);
    }
    public  IEnumerator LoadSaveDataScene()
    {
        data = SaveSystem.LoadData();

        var asyncOperation = SceneManager.LoadSceneAsync(data.currentSceneName);

        //asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //if (asyncOperation.progress >= 0.9f)
            //{
            //    Debug.Log("Scene is nearly loaded, activating...");
            //    asyncOperation.allowSceneActivation = true;
            //}
            yield return null;
        }
        Debug.Log("Scene loaded. Executing post-load operations.");
    }
    public void LoadSaveData()
    {
        Vector3 pos = new Vector3(data._playerPosition[0], data._playerPosition[1], data._playerPosition[2]);
        PlayerController.Instance.gameObject.transform.position = pos;

        UIInformationDisplay.Instance.LoadInformations(data.unlockedInformationsSO);
        UiThoughtPanel.Instance.LoadThoughts(data.thoughts);
        UiThoughtPanel.Instance.LoadConnections(data.lineConnectionGuids);
    }
}
