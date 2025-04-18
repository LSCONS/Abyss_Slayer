using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeDamageCheck : MonoBehaviour
{
    private float aliveTime = 0f;
    private BoxCollider2D boxCollider;
    private float damage = 10f;
    public System.Type effectType = null;      // typeof가 effectType

    private float colliderDuration = 0f;

    private LayerMask includeLayer;

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // 스킬 맞으면 여기다가 추가해서 중복 데미지 들어오지 않도록 막음

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        includeLayer = LayerData.GroundPlaneLayerMask;
    }

    /// <summary>
    /// 위치, 크기, 데미지, 이펙트 타입 설정
    /// </summary>
    /// <param name="sizeX">콜라이더 크기 X</param>
    /// <param name="sizeY">콜라이더 크기 Y</param>
    /// <param name="damage">데미지</param>
    /// <param name="effectType">이펙트 타입</param>
    /// <param name="aliveTime">이펙트 지속 시간</param>
    public void Init(float sizeX, float sizeY, float damage, System.Type effectType, float aliveTime)
    {
        boxCollider.size = new Vector2(sizeX, sizeY);
        this.damage = damage;
        this.effectType = effectType;
        this.aliveTime = aliveTime;
        hitObjects.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 맞은 대상이면 무시하기
        if(hitObjects.Contains(collision.gameObject))
            return;
        hitObjects.Add(collision.gameObject);

        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            boss.Damage((int)damage); // 데미지 전달
    
            if(effectType !=null)
            {
                BasePoolable effect = PoolManager.Instance.Get(effectType);
                if (effect != null)
                {
                    effect.transform.position = boss.transform.position;
                    effect.transform.SetParent(boss.transform);
                    effect.Init();
                    effect.AutoReturn(aliveTime);
                }
            }
        }

        if (((1 << collision.gameObject.layer) & includeLayer) != 0)
        {
            var poolable = GetComponent<BasePoolable>();
            if (poolable != null)
                poolable.ReturnToPool();
            else
                Destroy(gameObject); // 풀 객체가 아니면 그냥 파괴
        }
    }


}
