using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BaseSO", menuName = "ScriptableObjects/BaseSO", order = 1)]
public class BaseSO: ScriptableObject
{
    [field: SerializeField] public Vector2 Position;
    [field: SerializeField] public Guid ID;

    public  void Initialize()
    {
        ID = Guid.NewGuid();
    }
    public void Save(Vector2 position)
    {
        Position = position;
    }
}
public class InformationSO : ScriptableObject
{

}
public class NoteSO : ScriptableObject
{

}
public class DialogueFragment
{

}