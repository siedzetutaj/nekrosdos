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
    public override bool Equals(object obj)
    {
        if (obj is ConnectedThoughtsGuid other)
        {
            return (Id1 == other.Id1 && Id2 == other.Id2) ||
                   (Id1 == other.Id2 && Id2 == other.Id1);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id1.GetHashCode() ^ Id2.GetHashCode();
    }
    public bool IsThere(SerializableGuid id)
    {
        if (id == Id1 || id == Id2)  
            return true;
        return false;
    }
}
