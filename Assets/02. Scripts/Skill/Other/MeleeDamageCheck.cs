using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class MeleeDamageCheck : MonoBehaviour
{
    public BoxCollider2D BoxCollider { get; private set; }
    public Dictionary<GameObject, float> NextHitTime { get; private set; } = new();    // 맞은 시간 저장하는 딕셔너리
    public HashSet<GameObject> hitObjects { get; private set; } = new HashSet<GameObject>(); // 스킬 맞으면 여기다가 추가해서 중복 데미지 들어오지 않도록 막음
    public MeleeDamageCheckData Data { get; private set; }

    private void OnValidate()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnDisable()
    {
        hitObjects.Clear();
        NextHitTime.Clear();
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
    public void Init(MeleeDamageCheckData data)
    {
        if(BoxCollider == null)
            BoxCollider = GetComponent<BoxCollider2D>();
        Data = data;
        float flag = Data.Player.IsFlipX ? -1f : 1f;

        BoxCollider.size = Data.ColliderSize;
        BoxCollider.offset = new Vector2(Data.ColliderOffset.x * flag, Data.ColliderOffset.y);

        hitObjects.Clear();
        NextHitTime.Clear();
    }


    public void Init(MeleeDamageCheckData data, float duration)
    {
        Init(data);
        // 콜라이더 온오프
        data.Player.PlayerMeleeCollider.enabled = true;
        data.Player.StartCoroutine(data.Player.EnableMeleeCollider(data.Player.PlayerMeleeCollider, duration));
    }
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (Data.CanRepeatHit)   // 진입하자마자 딜링
            TryHit(col.gameObject);
        else
            TryHit(col.gameObject);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (Data.CanRepeatHit)
            TryHit(col.gameObject);
    }

    private void TryHit(GameObject target)
    {
        if (((1 << target.layer) & Data.TargetLayer) == 0) return;

        //Debug.Log($"TryHit: {target.name} at {Time.time}");

        // 반복 아니면 한 번만 처리하기
        if (!Data.CanRepeatHit)
        {
            if (hitObjects.Contains(target)) return;
            hitObjects.Add(target);
        }
        // 반복 아니면 다단 히트
        else
        {
            if (NextHitTime.TryGetValue(target, out float allowTime))
            {
                if (Time.time < allowTime) return; // 아직 쿨타임 안지났으면 return
            }

            NextHitTime[target] = Time.time + Data.TickRate; // 다음 가능 시간 기록
        }

        // 뎀지 처리
        if (target.TryGetComponent<IHasHealth>(out IHasHealth enemy))
        {
            enemy.Damage((int)Data.Damage, transform.position.x); // 데미지 전달
            Debug.Log($"Damage Applied at {Time.time}");

            Data.Skill.AttackAction?.Invoke();    // 스킬이 적중하면 플레이어한테 알려줌

            if (Data.EffectType != null)
            {
                BasePoolable effect = PoolManager.Instance.Get(Data.EffectType);
                if (effect != null)
                {
                    if (enemy is MonoBehaviour mb)
                    {
                        Transform enemyTransform = mb.transform;
                        effect.transform.position = enemyTransform.transform.position;
                        effect.transform.SetParent(enemyTransform);
                        effect.Init();
                        effect.AutoReturn(Data.Duration);
                    }

                }
            }
        }

        // 단타이면 콜라이더 종료
        if (!Data.CanRepeatHit)
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
        if (BoxCollider == null)
            BoxCollider = GetComponent<BoxCollider2D>();

        // 사이즈나 오프셋 적용된 박스 정보
        Vector3 worldCenter = transform.TransformPoint(BoxCollider.offset);
        // 로컬 크기를 월드 벡터로 변환
        Vector3 worldSize = transform.TransformVector(BoxCollider.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }

}


public class MeleeDamageCheckData
{
    public Player Player            { get; private set; }
    public Skill Skill              { get; private set; }
    public Type EffectType          { get; private set; }
    public Vector2 ColliderSize     { get; private set; }
    public Vector2 ColliderOffset   { get; private set; }
    public LayerMask TargetLayer    { get; private set; }
    public float Damage             { get; private set; }
    public float Duration           { get; private set; }
    public float TickRate           { get; private set; }
    public bool CanRepeatHit        { get; private set; } 
    public MeleeDamageCheckData(RemoteZoneRangeSkill _remoteZoneRangeSkill, Type _effectType)
    {
        Player = _remoteZoneRangeSkill.player;
        Skill = _remoteZoneRangeSkill;
        ColliderSize = _remoteZoneRangeSkill.ColliderSize;
        ColliderOffset = _remoteZoneRangeSkill.ColliderOffset;
        TargetLayer = _remoteZoneRangeSkill.TargetLayer;
        Damage = _remoteZoneRangeSkill.Damage;
        Duration = _remoteZoneRangeSkill.ColliderDuration;
        TickRate = _remoteZoneRangeSkill.TickRate;
        CanRepeatHit = _remoteZoneRangeSkill.CanRepeatHit;
        EffectType = _effectType;
    }

    public MeleeDamageCheckData
        (
            Player _player,  
            Skill _skill, 
            Vector2 _size, 
            Vector2 _offset, 
            LayerMask _layer, 
            float _damage, 
            float _duration,
            float _tickRate,
            bool _canRepeatHit,
            Type _effectType
        )
    {
        Player = _player;
        Skill = _skill;
        ColliderSize = _size;
        ColliderOffset = _offset;
        TargetLayer = _layer;
        Damage = _damage;
        Duration = _duration;
        TickRate = _tickRate;
        CanRepeatHit = _canRepeatHit;
        EffectType = _effectType;
    }
}
