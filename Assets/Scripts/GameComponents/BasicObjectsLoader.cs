using Cinemachine;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectsLoader : MonoBehaviourSingleton<BasicObjectsLoader>
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _eventSystem;
    [SerializeField] private GameObject _gameComponents;
    [SerializeField] private GameObject _loadDataHelper;

    [SerializeField] private Transform playerSpawnPos;
    private Vector3 zero = Vector3.zero;
    private Vector3 cameraPos = new Vector3(0, 0, -10);
    // Metoda wywo³ywana przy uruchomieniu sceny
    public void Start()
    {
        // Tworzenie obiektów na scenie
        Instantiate(_player, playerSpawnPos.transform.position, Quaternion.identity);

        GameObject MainCamera = Instantiate(_mainCamera, cameraPos, Quaternion.identity);
        MainCamera.GetComponentInChildren<CinemachineVirtualCamera>().Follow = PlayerController.Instance.gameObject.transform;

        GameObject CanvasUI = Instantiate(_ui, zero, Quaternion.identity);
        CanvasUI.GetComponent<Canvas>().worldCamera = Camera.main;
        Instantiate(_eventSystem, zero, Quaternion.identity);
        Instantiate(_gameComponents, zero, Quaternion.identity);

        DialogueDisplay.Instance.Initialize();

        PlayerController.Instance.Initialize();

        UIInformationDisplay.Instance.Initialize();

        UiThoughtPanel.Instance.Initialize();
        
        if (LoadDataHelper.Instance == null)
            Instantiate(_loadDataHelper, zero, Quaternion.identity);
        else
            LoadDataHelper.Instance.LoadSaveData();
    }
}
