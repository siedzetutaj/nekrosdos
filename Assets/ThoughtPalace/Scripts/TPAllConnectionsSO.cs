using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "TPConnectionsSO", menuName = "ScriptableObjects/TAllPConnections", order = 1)]

public class TPAllConnectionsSO : ScriptableSingleton<TPAllConnectionsSO>
{
    [SerializeField] public List<TPThoughtConections> AllConnections = new List<TPThoughtConections>();

}
