using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationInteraction : SampleInteraction
{
    [SerializeField] private SceneNames _sceneToLoad;
    public override void Interaction()
    {
        SceneManager.LoadScene(_sceneToLoad.ToString());
    }
}
