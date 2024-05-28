using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DisjointSet<T>
{
    [SerializeField] private Dictionary<T, T> parent;
    [SerializeField] private Dictionary<T, int> rank;

    public DisjointSet()
    {
        parent = new Dictionary<T, T>();
        rank = new Dictionary<T, int>();
    }

    public void MakeSet(T element)
    {
        if (!parent.ContainsKey(element))
        {
            parent[element] = element;
            rank[element] = 0;
        }
    }

    public T Find(T element)
    {
        if (!parent.ContainsKey(element))
            return default(T);

        if (!parent[element].Equals(element))
            parent[element] = Find(parent[element]);

        return parent[element];
    }

    public void Union(T x, T y)
    {
        T rootX = Find(x);
        T rootY = Find(y);

        if (rootX == null || rootY == null)
            return;

        if (rootX.Equals(rootY))
            return;

        if (rank[rootX] < rank[rootY])
            parent[rootX] = rootY;
        else if (rank[rootX] > rank[rootY])
            parent[rootY] = rootX;
        else
        {
            parent[rootY] = rootX;
            rank[rootX]++;
        }
    }

    public IEnumerable<T> GetAllParents()
    {
        foreach (var entry in parent)
        {
            if (entry.Key.Equals(entry.Value))
            {
                yield return entry.Key;
            }
        }
    }
}
