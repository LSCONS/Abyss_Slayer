using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectPool : MonoBehaviour
{
    [SerializeField] protected BasePoolable prefab;
    [SerializeField] protected int initialSize = 2;

    protected Queue<BasePoolable> poolQueue = new Queue<BasePoolable>();

    protected virtual void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    protected BasePoolable CreateNewObject()
    {
        BasePoolable obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        obj.SetPool(this);
        poolQueue.Enqueue(obj);
        return obj;
    }


    public BasePoolable Get()
    {
        if (poolQueue.Count == 0)
        {
            CreateNewObject(); // 필요시 자동 확장
        }

        BasePoolable obj = poolQueue.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(BasePoolable obj)
    {
        poolQueue.Enqueue(obj);
    }
}
