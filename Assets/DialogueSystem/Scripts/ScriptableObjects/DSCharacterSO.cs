using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SampleCharacter", menuName = "ScriptableObjects/DSCharacter", order = 1)]
public class DSCharacterSO : ScriptableObject
{
    [field: SerializeField] public List<Sprite> Emotions {  get; set; }
}
