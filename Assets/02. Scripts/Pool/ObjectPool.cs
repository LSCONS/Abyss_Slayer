using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : BasePoolable
{
    private Queue<T> pool = new();
    private T _prefab;
    private Transform _parent;

    public ObjectPool(T prefab, int initialSize, Transform parents)
    {
        _prefab = prefab;
        _parent = parents;

        for(int i = 0; i < initialSize; i++)
        {
            CreatNew();
        }
    }

    T CreatNew()
    {
        T obj = Object.Instantiate(_prefab, _parent);
        obj.SetPool(this as ObjectPool<BasePoolable>);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public T Get()
    {
        if(pool.Count == 0)
        {
            CreatNew();
        }

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void ReturnToPool(T obj)
    {
        pool.Enqueue(obj);
    }
}
