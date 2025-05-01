using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePoolable : MonoBehaviour
{
    protected ObjectPool<BasePoolable> _pool;

    //풀 설정
    public virtual void SetPool(ObjectPool<BasePoolable> pool)
    {
        _pool = pool;
    }

    //반드시 오버로딩하여 사용
    public abstract void Init();


    // aliveTime 후에 풀에 반환
    public virtual void AutoReturn(float aliveTime){}

    public virtual void ReturnToPool()
    {
        gameObject.SetActive(false);
        _pool.ReturnToPool(this);
    }
}
