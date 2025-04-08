using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileNormalPool : MonoBehaviour
{
    [SerializeField] private BossProjectileNormal prefab;
    [SerializeField] private int initialSize = 12;

    private Queue<BossProjectileNormal> poolQueue = new Queue<BossProjectileNormal>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private BossProjectileNormal CreateNewObject()
    {
        BossProjectileNormal obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        obj.SetPool(this);
        poolQueue.Enqueue(obj);
        return obj;
    }
    /// <summary>
    /// 일반탄환 생성(정해진방향,속도로일정하게 진행되는 탄환)
    /// </summary>
    /// <param name="position">탄환생성좌표</param>
    /// <param name="direction">탄환진행방향</param>
    /// <param name="speed">탄환속도, 기본값=1</param>
    /// <param name="delayTime">탄스폰후 발사까지 시간차, 기본값 0초</param>
    /// <param name="size">탄 사이즈, 기본값 1</param>
    /// <param name="spriteNum">탄 모양, 기본값 0</param>
    public void Spawn(Vector3 position,Vector3 direction,float speed = 1f,float delayTime = 0f, float size = 1f, int spriteNum = 0)
    {
        if (poolQueue.Count == 0)
        {
            CreateNewObject(); // 필요시 자동 확장
        }

        BossProjectileNormal obj = poolQueue.Dequeue();
        obj.gameObject.SetActive(true);
        obj.Init(position, direction, speed, delayTime, size, spriteNum);
    }

    public void ReturnToPool(BossProjectileNormal obj)
    {
        poolQueue.Enqueue(obj);
    }
}
