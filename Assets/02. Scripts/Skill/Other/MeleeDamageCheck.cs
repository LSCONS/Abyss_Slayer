using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
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

    private Skill skill; // 어떤 스킬이 이 데미지체크를 사용했는가
    private Player player;


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
    public void Init(Vector2 size, float damage, System.Type effectType, float aliveTime)
    {
        boxCollider.size = size;
        this.damage = damage;
        this.effectType = effectType;
        this.aliveTime = aliveTime;
        
        hitObjects.Clear();
        nextHitTime.Clear();
    }


    /// <summary>
    /// 위치, 크기, 데미지, 이펙트 타입, 이펙트 타임 설정
    /// </summary>
    /// <param name="sizeX">콜라이더 크기 X</param>
    /// <param name="sizeY">콜라이더 크기 Y</param>
    /// <param name="offset">오프셋</param>
    /// <param name="damage">데미지</param>
    /// <param name="effectType">이펙트 타입</param>
    /// <param name="aliveTime">이펙트 지속 시간</param>
    public void Init(Player player, Vector2 size, Vector2 offset, float damage, System.Type effectType, float aliveTime)
    {
        this.player = player;

        boxCollider.size = size;
        boxCollider.offset = offset;
        this.damage = damage;
        this.effectType = effectType;
        this.aliveTime = aliveTime;


        float flag = 1f;

        if(player.SpriteRenderer !=null) flag = player.SpriteRenderer.flipX ? -1f : 1f;
        boxCollider.offset = new Vector2(offset.x * flag, offset.y);

        hitObjects.Clear();
        nextHitTime.Clear();
    }

    public void Init(Player player, Skill skill, Vector2 size, Vector2 offset, float damage, System.Type effectType, float aliveTime)
    {
        this.skill = skill;
        Init(player, size, offset, damage, effectType, aliveTime);
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

            player.RaiseSkillHit(skill);    // 스킬이 적중하면 플레이어한테 알려줌

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
    // 공격 기즈모로 확인해보기
    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        // 사이즈나 오프셋 적용된 박스 정보
        Vector3 worldCenter = transform.TransformPoint(boxCollider.offset);
        // 로컬 크기를 월드 벡터로 변환
        Vector3 worldSize = transform.TransformVector(boxCollider.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }

}
