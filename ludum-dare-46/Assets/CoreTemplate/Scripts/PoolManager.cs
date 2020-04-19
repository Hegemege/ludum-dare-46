using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities;

public class PoolManager : GenericManager<PoolManager>, ILoadedManager
{
    [HideInInspector]
    public List<EnvironmentSpawnablePool> EnvironmentSpawnables;

    public void Initialize()
    {
        if (!InitializeSingleton(this)) return;

        EnvironmentSpawnables = GetComponentsInChildren<EnvironmentSpawnablePool>().ToList();
    }

    public void PostInitialize() { }

    public void ResetPools()
    {
        foreach (var environmentPool in EnvironmentSpawnables)
        {
            environmentPool.Pool.Clear();
        }
    }

    public GameObject GetRandomEnvironmentSpawnable()
    {
        var index = Random.Range(0, EnvironmentSpawnables.Count);
        return EnvironmentSpawnables[index].GetPooledObject().gameObject;
    }
}