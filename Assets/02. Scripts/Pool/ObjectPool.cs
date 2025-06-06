using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public Queue<BasePoolable> QuePool { get; private set; } = new();
    public List<BasePoolable> ListAllPoolable { get; private set; } = new();
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
        BasePoolable obj = RunnerManager.Instance.GetRunner().Spawn
            (
            _prefab,
            Vector3.zero,
            Quaternion.identity,
            RunnerManager.Instance.GetRunner().LocalPlayer,
            (runner, obj) =>
            {
                BasePoolable basePoolable = obj.GetComponent<BasePoolable>();
                basePoolable._pool = this;
                basePoolable.transform.parent = null;
                basePoolable.transform.parent = _parent;
                basePoolable.SetPool(this);
                basePoolable.gameObject.SetActive(false);
                QuePool.Enqueue(basePoolable);
                ListAllPoolable.Add(basePoolable);
            });
        return obj;
    }

    //프리펩을 (생성)활성화, 해당 프리펩의 스크립트를 제네릭T를 이용해 반환
    public BasePoolable Get()      
    {
        if(QuePool.Count == 0)
        {
            CreatNew();
        }

        BasePoolable obj = QuePool.Dequeue();
        return obj;
    }


    public void ReturnToPool(BasePoolable obj)
    {
        QuePool.Enqueue(obj);
    }
}
