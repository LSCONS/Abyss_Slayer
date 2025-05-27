using Fusion;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class MeleeDamageCheck : MonoBehaviour
{
    public BoxCollider2D BoxCollider { get; set; }
    public Dictionary<GameObject, float> NextHitTime { get; private set; } = new();    // 맞은 시간 저장하는 딕셔너리
    public HashSet<GameObject> hitObjects { get; private set; } = new HashSet<GameObject>(); // 스킬 맞으면 여기다가 추가해서 중복 데미지 들어오지 않도록 막음
    public Coroutine ColliderStartCoroutine { get; private set; }
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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(MeleeDamageCheckData data)
    {
        if(BoxCollider == null) BoxCollider = GetComponent<BoxCollider2D>();
        Data = data;

        float flag = ServerManager.Instance.DictRefToPlayer[Data.PlayerRef].IsFlipX ? -1f : 1f;

        BoxCollider.size = Data.ColliderSize;
        BoxCollider.offset = new Vector2(Data.ColliderOffset.x * flag, Data.ColliderOffset.y);

        hitObjects.Clear();
        NextHitTime.Clear();
        ColliderStartCoroutine = StartCoroutine(SetColliderDelay(Data.StartDelayTime));
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(MeleeDamageCheckData data, float flipX)
    {
        if (BoxCollider == null)
            BoxCollider = GetComponent<BoxCollider2D>();
        float flag = flipX;
        Data = data;

        BoxCollider.size = Data.ColliderSize;
        BoxCollider.offset = new Vector2(Data.ColliderOffset.x * flag, Data.ColliderOffset.y);

        hitObjects.Clear();
        NextHitTime.Clear();
        ColliderStartCoroutine = StartCoroutine(SetColliderDelay(Data.StartDelayTime));
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_BasicInit(MeleeDamageCheckData data)
    {
        Rpc_Init(data);
        StartCoroutine(ExitColliderDelay(data.ColliderDuration));
    }


    /// <summary>
    /// 콜라이더의 공격을 끝내고 비활성화 시키는 메서드
    /// </summary>
    public void Exit()
    {
        if(ColliderStartCoroutine != null)
            StopCoroutine(ColliderStartCoroutine);
        ColliderStartCoroutine = null;
        BoxCollider.enabled = false;
    }


    /// <summary>
    /// 특정 딜레이 시간 이후 콜라이더를 활성화시키고 공격하는 메서드
    /// </summary>
    /// <param name="satrtDelayTime"></param>
    /// <returns></returns>
    private IEnumerator SetColliderDelay(float satrtDelayTime)
    {
        yield return new WaitForSeconds(satrtDelayTime);
        BoxCollider.enabled = true;
        ColliderStartCoroutine = null;
    }


    /// <summary>
    /// 특정 시간 이후 콜라이더를 비활성화시키는 메서드
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator ExitColliderDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Exit();
    }


    private void OnTriggerStay2D(Collider2D col)
    {
        TryHit(col.gameObject);
    }


    private void TryHit(GameObject target)
    {
        Debug.Log("레이어 확인");
        Debug.Log("1. " + (1 << target.layer));
        Debug.Log("2. " + Data.TargetLayer);
        Debug.Log("3. " + (LayerMask)Data.TargetLayer);
        if (((1 << target.layer) & (LayerMask)Data.TargetLayer) == 0) return;
        Debug.Log("레이어 통과");

        if (NextHitTime.TryGetValue(target, out float allowTime))
        {
            Debug.Log($"1. {Time.time}");
            Debug.Log($"2. {allowTime}");
            Debug.Log("숫자 올라가용");
            if (Time.time < allowTime) return; // 아직 쿨타임 안지났으면 return
        }

        Debug.Log("숫자 끝났어용");
        NextHitTime[target] = Time.time + Data.TickRate; // 다음 가능 시간 기록

        // 뎀지 처리
        if (target.TryGetComponent<IHasHealth>(out IHasHealth enemy))
        {
            Debug.Log("타겟 찾았어용");
            if (DataManager.Instance.DictEnumToType.TryGetValue((EType)Data.ClassTypeInt, out Type type))
            {
                Debug.Log("타겟 히트 이펙트 있어용");
                BasePoolable effect = PoolManager.Instance.Get(type);
                if (effect != null && enemy is MonoBehaviour mb)
                {
                    Transform enemyTransform = mb.transform;
                    effect.transform.position = enemyTransform.transform.position;
                    effect.transform.SetParent(enemyTransform);
                    effect.Rpc_Init();

                    // 히트 이펙트 출력
                    if (effect is BossHitEffect bhe)
                    {
                        bhe.PlayEffect((EHitEffectType)Data.HitEffectInt);
                    }
                    effect.AutoReturn(Data.ColliderDuration);
                }
            }


            Debug.Log("타겟 데미지 줄게용");
            if (Data.GetSkill().SkillCategory == SkillCategory.DashAttack)
            {
                ServerManager.Instance.DictRefToPlayer[Data.PlayerRef].StartCoroutine(AttackEnemyCombo(enemy, 0.1f, 10));
            }
            else
            {
                AttackEnemy(enemy);
            }
        }
    }

    private void AttackEnemy(IHasHealth enemy)
    {
        Debug.Log("타겟 데미지 주고 있어용");
        enemy.Rpc_Damage((int)(Data.Damage * ServerManager.Instance.DictRefToPlayer[Data.PlayerRef].DamageValue.Value), ServerManager.Instance.DictRefToPlayer[Data.PlayerRef].transform.position.x); // 백어택 계산하는 데미지 전달
        Data.GetSkill().AttackAction?.Invoke();    // 스킬이 적중하면 플레이어한테 알려줌
    }

    private IEnumerator AttackEnemyCombo(IHasHealth enemy, float delayTime, int attackCount)
    {
        for(int i = 0; i < attackCount; i++)
        {
            AttackEnemy(enemy);
            yield return new WaitForSeconds(delayTime);
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


public struct MeleeDamageCheckData : INetworkStruct
{
    public PlayerRef PlayerRef            { get; private set; }
    public Vector2 ColliderSize     { get; set; }
    public Vector2 ColliderOffset   { get; set; }
    public int TargetLayer    { get; private set; }
    public float StartDelayTime          {  get; private set; }
    public float Damage             { get; private set; }
    public float ColliderDuration           { get; private set; }
    public float TickRate           { get; private set; }
    public int SkillSlotKeyInt { get; private set; }
    public int AnimatorControllerInt { get; private set; }
    public int HitEffectInt { get; private set; }  // 히트 이펙트 정보
    public int ClassTypeInt { get; private set; }
    public MeleeDamageCheckData(RemoteZoneRangeSkill _remoteZoneRangeSkill, int _eHitEffectTypeInt, int _classTpyeInt)
    {
        PlayerRef = _remoteZoneRangeSkill.player.PlayerRef;
        SkillSlotKeyInt = (int)_remoteZoneRangeSkill.slotKey;
        ColliderSize = _remoteZoneRangeSkill.ColliderSize;
        ColliderOffset = _remoteZoneRangeSkill.ColliderOffset;
        TargetLayer = _remoteZoneRangeSkill.TargetLayer;
        StartDelayTime = _remoteZoneRangeSkill.ColliderSetDelayTime;
        Damage = _remoteZoneRangeSkill.Damage;
        ColliderDuration = _remoteZoneRangeSkill.ColliderDuration;
        TickRate = _remoteZoneRangeSkill.TickRate;
        AnimatorControllerInt = (int)_remoteZoneRangeSkill.EEffectAnimatorController;
        HitEffectInt = _eHitEffectTypeInt;
        ClassTypeInt = _classTpyeInt;
    }

    public MeleeDamageCheckData
        (
            PlayerRef _playerRef,
            int _skillSlotKeyInt,
            Vector2 _size,
            Vector2 _offset,
            int _layer,
            float _delayTime,
            float _damage,
            float _duration,
            float _tickRate,
            int _animatorControllerInt,
            int _hitEffectInt = -1,
            int _classTpyeInt = (int)EType.None
        )
    {
        PlayerRef = _playerRef;
        SkillSlotKeyInt = _skillSlotKeyInt;
        ColliderSize = _size;
        ColliderOffset = _offset;
        TargetLayer = _layer;
        StartDelayTime = _delayTime;
        Damage = _damage;
        ColliderDuration = _duration;
        TickRate = _tickRate;
        AnimatorControllerInt = _animatorControllerInt;
        HitEffectInt = _hitEffectInt;
        ClassTypeInt = _classTpyeInt;
    }

    public Skill GetSkill()
    {
        return ServerManager.Instance.DictRefToPlayer[PlayerRef].DictSlotKeyToSkill[(SkillSlotKey)SkillSlotKeyInt];
    }

    public void SetCollider(Vector2 colliderSize, Vector2 colliderOffset)
    {
        ColliderSize = colliderSize;
        ColliderOffset = colliderOffset;
    }
}
