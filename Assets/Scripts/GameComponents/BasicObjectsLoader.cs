using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectsLoader : MonoBehaviour
{
    [SerializeField] private GameObject _eventSystem;
    [SerializeField] private GameObject _gameComponents;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _ui;

    [SerializeField] private Transform playerSpawnPos;
    private Vector3 zero = Vector3.zero;
    private Vector3 cameraPos = new Vector3(0, 0, -10);
    // Metoda wywo³ywana przy uruchomieniu sceny
    void Start()
    {
        // Tworzenie obiektów na scenie
        Instantiate(_player, playerSpawnPos.transform.position, Quaternion.identity);
        Instantiate(_ui, zero, Quaternion.identity);
        Instantiate(_eventSystem, zero, Quaternion.identity);
        Instantiate(_gameComponents, zero, Quaternion.identity);
        Instantiate(_mainCamera, cameraPos, Quaternion.identity);
    }
}
