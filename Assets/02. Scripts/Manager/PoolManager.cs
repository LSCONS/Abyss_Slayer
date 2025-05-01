using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Fusion.Allocator;

[Serializable]
public class PoolConfig     //새로 풀을 추가할때 필요한 정보
{
    public BasePoolable prefab;     //오브젝트풀 구조로 관리할 프리펩
    public Transform parent;        //해당 오브젝트를 생성할 부모
    public int initialSize;
}

/// <summary>
/// 
/// </summary>
public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<Type, ObjectPool<BasePoolable>> poolDict = new();    //리스트의 풀을 저장하는 딕셔너리


    protected override void Awake()
    {
        base.Awake();
        SetObjectPoolDictionary();
    }


    /// <summary>
    /// PoolDictionary를 초기화
    /// </summary>
    private void SetObjectPoolDictionary()
    {
        BasePoolable[] basePoolables = GetAllPoolable();
        foreach(BasePoolable poolable in basePoolables)
        {
            //이미 들어있는 타입인 경우 다음으로 넘어감.
            if (poolDict.ContainsKey(poolable.GetType())) continue;

            //빈 오브젝트 추가 후 부모 오브젝트 설정
            Transform newObject = new GameObject(poolable.GetType().Name).transform;
            newObject.parent = transform;

            //TODO: 임시로 풀링할 오브젝트 5개씩 모두 생성후 Dictionary에 추가
            ObjectPool<BasePoolable> pool = new ObjectPool<BasePoolable>(poolable, newObject, 5);
            poolDict.Add(poolable.GetType(), pool);
        }
    }


    /// <summary>
    /// Resources/ObjectPoolPrefab에 들어있는 모든 풀 오브젝트를 로드한 뒤 반환
    /// </summary>
    private BasePoolable[] GetAllPoolable()
    {
        //TODO: 나중에 Resources 어드레서블로 교체 필요
        GameObject[] poolObjectArray = Resources.LoadAll<GameObject>("ObjectPoolPrefab");
        BasePoolable[] basePoolables = new BasePoolable[poolObjectArray.Length];
        for (int i = 0; i < poolObjectArray.Length; i++)
        {
            basePoolables[i] = poolObjectArray[i].GetComponent<BasePoolable>();
            if (basePoolables[i] == null) Debug.LogError("PoolObject BasePoolable is null");
        }
        return basePoolables;
    }

    /// <summary>
    /// 오브젝트풀로 관리되는 오브젝트를 생성,호출 ==>> 반드시 Init(초기화)호출 필요
    /// </summary>
    /// <typeparam name="T">가져올 오브젝트 클래스</typeparam>
    /// <returns></returns>
    public T Get<T>() where T : BasePoolable            //제네릭T 에 받은 생성할 프리펩의 클래스를 키값으로 딕셔너리에서 오브젝트풀을 불러와 Get을 호출하여 입력된 T값에 맞는 프리펩클래스를 반환
    {
        var type = typeof(T);       //입력받은 제네릭T 값을 키값으로 정의

        if (poolDict.TryGetValue(type, out var pool))   //키값으로 딕셔너리의 오브젝트풀을 호출
            return (T)pool.Get();                       //호출한 오브젝트풀에서 오브젝트를 생성,호출

            Debug.LogWarning($"{type}에 대한 풀을 찾을 수 없습니다.");
        return null;
    }


    /// <summary>
    /// 풀 타입을 받아 풀에서 오브젝트를 가져오는 메서드
    /// </summary>
    /// <param name="type">풀 타입</param>
    /// <returns>풀에서 가져온 오브젝트</returns>
    public BasePoolable Get(System.Type type)
    {
        if (poolDict.TryGetValue(type, out var pool))
            return pool.Get();

        Debug.LogWarning($"{type}에 대한 풀을 찾을 수 없습니다.");
        return null;
    }
}
