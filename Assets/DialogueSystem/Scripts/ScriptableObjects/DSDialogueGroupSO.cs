using UnityEngine;


public class DSDialogueGroupSO : ScriptableObject
{
    [field: SerializeField] public string GroupName { get; set; }
    [field: SerializeField] public string ID { get; set; }

    public void Initialize(string groupName, string id)
    {
        GroupName = groupName;
        ID = id;
    }
}
