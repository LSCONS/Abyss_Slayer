using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageCheck : MonoBehaviour
{
    private float aliveTime = 0f;
    private BoxCollider2D boxCollider;
    private float damage = 10f;
    public System.Type effectType = null;      // typeof가 effectType

    [SerializeField] private bool canRepeatHit = false; // 다단히트 할거임?
    private float repeatHitCoolTime = 0.5f;    // 반복 딜 쿨타임
    private Dictionary<GameObject, float> nextHitTime = new();    // 맞은 시간 저장하는 딕셔너리

    private LayerMask includeLayer;

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // 스킬 맞으면 여기다가 추가해서 중복 데미지 들어오지 않도록 막음

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        includeLayer = LayerData.EnemyLayerMask;
    }

    private void OnDisable()
    {
        hitObjects.Clear();
        nextHitTime.Clear();
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
        nextHitTime.Clear();


    }
    /// <summary>
    /// 반복 여부와 쿨타임 설정
    /// </summary>
    public void SetRepeatMode(bool repeat, float cooldown)
    {
        canRepeatHit = repeat;
        repeatHitCoolTime = cooldown;

        Debug.Log($"aliveTime: {aliveTime} // repeatHitCoolTime: {repeatHitCoolTime} ");

    }


    private void OnTriggerEnter2D(Collider2D col)
    {

        if (canRepeatHit)   // 진입하자마자 딜링
            TryHit(col.gameObject);
        else
            TryHit(col.gameObject);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log($"[Stay] {col.name} at {Time.time}");
        if (canRepeatHit)
            TryHit(col.gameObject);
    }

    private void TryHit(GameObject target)
    {
        if (((1 << target.layer) & includeLayer) == 0) return;

        //Debug.Log($"TryHit: {target.name} at {Time.time}");

        // 반복 아니면 한 번만 처리하기
        if (!canRepeatHit)
        {
            if (hitObjects.Contains(target)) return;
            hitObjects.Add(target);
        }
        // 반복 아니면 다단 히트
        else
        {
            if (nextHitTime.TryGetValue(target, out float allowTime))
            {
                if (Time.time < allowTime) return; // 아직 쿨타임 안지났으면 return
            }

            nextHitTime[target] = Time.time + repeatHitCoolTime; // 다음 가능 시간 기록
        }

            // 뎀지 처리
        if (target.TryGetComponent<Boss>(out Boss boss))
        {
            boss.Damage((int)damage); // 데미지 전달
            Debug.Log($"Damage Applied at {Time.time}");

            if (effectType != null)
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

        // 단타이면 콜라이더 종료
        if (!canRepeatHit)
        {
            var poolable = GetComponent<BasePoolable>();
            if (poolable != null)
                poolable.ReturnToPool();
            else return;

            //Destroy(gameObject, 10); // 풀 객체가 아니면 그냥 파괴
        }

    }

}
