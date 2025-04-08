using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    [SerializeField] private Explosion prefab;
    [SerializeField] private int initialSize = 2;

    private Queue<Explosion> poolQueue = new Queue<Explosion>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private Explosion CreateNewObject()
    {
        Explosion obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        obj.SetPool(this);
        poolQueue.Enqueue(obj);
        return obj;
    }
    /// <summary>
    /// 폭발이펙트 생성
    /// </summary>
    /// <param name="position">생성위치</param>
    /// <param name="size">폭발 사이즈, 기본1, 0이하 숫자입력시 기본값</param>
    /// <param name="spriteNum">폭발 이펙트 스프라이트 종류, 기본0,</param>
    /// <returns></returns>
    public void Get(Vector3 position,float size = 1,int spriteNum = 0)
    {
        if (poolQueue.Count == 0)
        {
            CreateNewObject(); // 필요시 자동 확장
        }

        Explosion obj = poolQueue.Dequeue();
        obj.gameObject.SetActive(true);
        obj.Init(position, size, spriteNum);
    }

    public void ReturnToPool(Explosion obj)
    {
        poolQueue.Enqueue(obj);
    }
}
