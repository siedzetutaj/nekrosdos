using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    Vector3 position; 

    public SaveData() 
    { 
        position = PlayerController.Instance.gameObject.transform.position;
    }
}
