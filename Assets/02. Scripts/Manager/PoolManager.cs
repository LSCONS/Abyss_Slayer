using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.Allocator;

[Serializable]
public class PoolConfig
{
    public BasePoolable prefab;
    public Transform parent;
    public int initialSize = 10;
}
public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private List<PoolConfig> poolConfigs;
    private Dictionary<Type, ObjectPool<BasePoolable>> poolDict = new();


    public BaseObjectPool explosionPool;
    public BaseObjectPool bossProjectileNormalPool;

    protected override void Awake()
    {
        base.Awake();

        for(int i = 0; i < poolConfigs.Count; i++)
        {
            ObjectPool<BasePoolable> pool = new ObjectPool<BasePoolable>(poolConfigs[i].prefab, poolConfigs[i].initialSize, poolConfigs[i].parent);
            poolDict.Add(poolConfigs[i].GetType(), pool);
        }
    }
    public T Get<T>() where T : BasePoolable
    {
        var type = typeof(T);

        if (poolDict.TryGetValue(type, out var pool))
            return (T)pool.Get();

        Debug.LogWarning($"{type}에 대한 풀을 찾을 수 없습니다.");
        return null;
    }
}
