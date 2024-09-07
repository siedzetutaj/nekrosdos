using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;

public class LocationInteraction : SampleInteraction
{
    [SerializeField] private SceneNames _sceneToLoad;
    public override void Interaction()
    {
        SaveSystem.SaveData();
        LoadDataHelper.Instance.isSwitchingScene = true;
        SceneManager.LoadSceneAsync(_sceneToLoad.ToString());
    }
}
