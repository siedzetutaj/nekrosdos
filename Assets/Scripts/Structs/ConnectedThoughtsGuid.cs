using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ConnectedThoughtsGuid
{
    [field: SerializeField] public SerializableGuid Id1;
    [field: SerializeField] public SerializableGuid Id2;
    public ConnectedThoughtsGuid(Guid id1, Guid id2)
    {
        Id1 = id1;
        Id2 = id2;
    }
}
