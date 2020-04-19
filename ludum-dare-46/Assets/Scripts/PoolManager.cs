using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtilities;

public class PoolManager : GenericManager<PoolManager>, ILoadedManager
{
    [HideInInspector]
    public List<EnvironmentSpawnablePool> EnvironmentSpawnables;

    [HideInInspector]
    public List<ObstacleSpawnablePool> ObstacleSpawnables;

    public ParticleSystemPool ExplosionParticlePool;
    public ParticleSystemPool DustParticlePool;
    public ParticleSystemPool DeathParticlePool;

    public void Initialize()
    {
        if (!InitializeSingleton(this)) return;

        EnvironmentSpawnables = GetComponentsInChildren<EnvironmentSpawnablePool>().ToList();
        ObstacleSpawnables = GetComponentsInChildren<ObstacleSpawnablePool>().ToList();
    }

    public void PostInitialize() { }

    public void ResetPools()
    {
        foreach (var environmentPool in EnvironmentSpawnables)
        {
            ResetPool(environmentPool);
        }

        foreach (var obstaclePool in ObstacleSpawnables)
        {
            ResetPool(obstaclePool);
        }

        ResetPool(ExplosionParticlePool);
        ResetPool(DustParticlePool);
        ResetPool(DeathParticlePool);
    }

    private void ResetPool<T>(GenericComponentPool<T> pool)
    {
        foreach (var obj in pool.Pool)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public GameObject GetRandomEnvironmentSpawnable()
    {
        var index = Random.Range(0, EnvironmentSpawnables.Count);
        return EnvironmentSpawnables[index].GetPooledObject().gameObject;
    }

    public GameObject GetRandomObstacle()
    {
        var index = Random.Range(0, ObstacleSpawnables.Count);
        return ObstacleSpawnables[index].GetPooledObject().gameObject;
    }
}