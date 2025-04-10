using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : BasePoolable
{
    private Queue<T> pool = new();
    private T _prefab;
    private Transform _parent;

    //생성자, 초기화 (프리펩,초기생성수,부모)
    public ObjectPool(T prefab, int initialSize, Transform parents) 
    {
        _prefab = prefab;
        _parent = parents;

        for(int i = 0; i < initialSize; i++)
        {
            CreatNew();
        }
    }

    T CreatNew()        //부족할경우 추가생성
    {
        T obj = Object.Instantiate(_prefab, _parent);
        obj.SetPool(this as ObjectPool<BasePoolable>);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public T Get()      //프리펩을 (생성)활성화, 해당 프리펩의 스크립트를 제네릭T를 이용해 반환
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
