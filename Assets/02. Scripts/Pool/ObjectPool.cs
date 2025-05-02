using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<BasePoolable> pool = new();
    private BasePoolable _prefab;
    private Transform _parent;

    //생성자, 초기화 (프리펩,초기생성수,부모)
    public ObjectPool(BasePoolable prefab, Transform parents, int initialSize)
    {
        _prefab = prefab;
        _parent = parents;

        for (int i = 0; i < initialSize; i++)
        {
            CreatNew();
        }
    }

    BasePoolable CreatNew()        //부족할경우 추가생성
    {
        if (_prefab == null)
        {
            Debug.LogError("ObjectPool 생성 시 prefab이 null입니다!");
            return null;
        }
        BasePoolable obj = Object.Instantiate(_prefab, _parent);
        obj.SetPool(this);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    //프리펩을 (생성)활성화, 해당 프리펩의 스크립트를 제네릭T를 이용해 반환
    public BasePoolable Get()      
    {
        if(pool.Count == 0)
        {
            CreatNew();
        }

        BasePoolable obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }


    public void ReturnToPool(BasePoolable obj)
    {
        pool.Enqueue(obj);
    }
}
