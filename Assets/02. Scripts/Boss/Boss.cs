using Cinemachine;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[RequireComponent(typeof(NetworkObject), typeof(BossController))]
public class Boss : NetworkBehaviour, IHasHealth
{
    [field: SerializeField] public BossController          BossController    { get; private set; }
    [field: SerializeField] public SpriteRenderer           Sprite            { get; private set; }
    [field: SerializeField] public Animator                 Animator          { get; private set; }
    [field: SerializeField] public Collider2D               HitCollider       { get; private set; } //보스 피격판정 콜라이더
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera     { get; private set; }
    [field: SerializeField] public ReactiveProperty<int>    MaxHp             { get; private set; } = new ReactiveProperty<int> (1000);
    [field: SerializeField] public bool                     IsRest            { get; set; }         = true;
    [field: SerializeField] public bool                     IsOpenSprtie     { get; set; }         = false;
    private Dictionary<DebuffType, DebuffData>              ActiveDebuffs     { get; set; }         = new();   // 디버프 상태를 저장              
    public ReactiveProperty<int>                            Hp                { get; private set; } = new ReactiveProperty<int>(1000);
    public bool                                             IsDead            { get; private set;}  = false;
    public float                                            DamageMultiplier  { get; set; }         = 1f;      // 보스가 받는 데미지 배율 (기본은 1.0)

    [field: Header("네트워크에 공유할 데이터들")]
    [Networked] public bool IsLeft { get; set; }
    [Networked] public Vector2 BossPosition { get; set; }
    [Networked] public Vector2 CrosshairPosition { get; set; }


    public override void Spawned()
    {
        base.Spawned();
        BossController.Init();
        Hp.Value = MaxHp.Value;
        ServerManager.Instance.Boss = this;
        Animator.enabled = IsOpenSprtie;
        Sprite.enabled = IsOpenSprtie;
    }


    private void Update()
    {
        if (!HasStateAuthority) return;
        
        ServerUpdate();
        ClientUpdate();
        if(Sprite.flipX != IsLeft) { Sprite.flipX = IsLeft; }
    }


    /// <summary>
    /// 서버에서 실행할 Update메서드
    /// </summary>
    private void ServerUpdate()
    {
        if (!(RunnerManager.Instance.GetRunner().IsServer)) return;

        //서버에서 현재의 보스 포지션을 공유
        if ((Vector2)transform.position != BossPosition)
        {
            BossPosition = (Vector2)transform.position;
        }

        if (BossController.ChasingTarget)
        {
            bool temp = BossController.Target.position.x - transform.position.x < 0;
            if (IsLeft != temp)
            {
                IsLeft = temp;
            }
        }

        if (!(BossController.ChasingTarget)) return;
        BossController.TargetCrosshair.position = BossController.Target.position;

        if(CrosshairPosition != (Vector2)BossController.Target.position)
        {
            CrosshairPosition = (Vector2)BossController.Target.position;
        }
    }


    /// <summary>
    /// 클라이언트에서 실행할 Update메서드
    /// </summary>
    private void ClientUpdate()
    {
        if (Runner.IsServer) return;

        //서버에서 공유 받은 SpriteFlipX를 적용
        if (Sprite.flipX != IsLeft)
        {
            Sprite.flipX = IsLeft;
        }

        //서버에서 공유 받은 포지션을 적용
        if ((Vector2)transform.position != BossPosition)
        {
            //TODO: 보간을 하며 점점 따라갈 수 있도록 만듦.
            transform.position = BossPosition;
        }

        if (!(BossController.ChasingTarget)) return;

        if (CrosshairPosition != (Vector2)BossController.TargetCrosshair.position)
        {
            BossController.TargetCrosshair.position = CrosshairPosition;
        }
    }


    public void ChangeHP(int value)
    {
        Hp.Value = Mathf.Clamp(Hp.Value + value, 0, MaxHp.Value);
        if (Hp.Value == 0)
        {
            IsDead = true;
            BossController.OnDead();
            if (Runner.IsServer) PoolManager.Instance.ReturnPoolAllObject();
        }
    }


    /// <summary>
    /// 애니메이터 ReSetTrigger를 공유할 Rpc 메서드
    /// </summary>
    /// <param name="hash">공유할 애니메이션 해시</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ResetTriggerAnimationHash(int hash)
    {
        Animator.ResetTrigger(hash);
    }


    /// <summary>
    /// 애니메이터 SetTrigger를 공유할 Rpc 메서드
    /// </summary>
    /// <param name="hash">공유할 애니메이션 해시</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetTriggerAnimationHash(int hash)
    {
        Animator.SetTrigger(hash);
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void Rpc_SetSpriteEnable(bool enable)
    {
        Animator.enabled = enable;
        Sprite.enabled = enable;
    }


    /// <summary>
    /// 애니메이터 SetBool을 공유하는 Rpc 메서드
    /// </summary>
    /// <param name="hash">공유할 애니메이션 해시</param>
    /// <param name="value">활성화 여부</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetBoolAnimationHash(int hash, bool value)
    {
        Animator.SetBool(hash, value);
    }


    /// <summary>
    /// 애니메이터 SetFloat를 공유하는 Rpc 메서드
    /// </summary>
    /// <param name="hash">공유할 애니메이션 해시</param>
    /// <param name="value">적용할 float 값</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetFloatAnimationHash(int hash, float value)
    {
        Animator.SetFloat(hash, value);
    }


    /// <summary>
    /// 애니메이터 SetInt를 공유하는 Rpc 메서드
    /// </summary>
    /// <param name="hash">공유할 애니메이션 해시</param>
    /// <param name="value">적용할 int 값</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetIntegerAniamtionHash(int hash, int value)
    {
        Animator.SetInteger(hash, value);
    }


    /// <summary>
    /// 데미지(양수) 입히기
    /// </summary>
    /// <param name="damage">입힐 데미지</param>
    /// <param name="attackPosX">데미지 입히는 주체의 위치X값</param>
    public void Damage(int damage, float attackPosX = -1000)
    {
        if (IsDead)
        {
            return;
        }

        float finalDamage = damage * DamageMultiplier;  // 데미지 배율 적용

        
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);        // 보스 위치 기준으로 worldPos 잡아주기 (데미지 인디케이터를 위한)

        if (attackPosX == -1000 || (attackPosX - transform.position.x < 0) == IsLeft)
        {
            ChangeHP(-(int)finalDamage);
            Damaged();
            Animator.SetTrigger(AnimationHash.DamagedParameterHash);
            DamageTextSpawner.Show((int)finalDamage, worldPos);                                             // 데미지 인디케이터 스폰
        }
        else
        {
            int totalFinalDamage = (int)(finalDamage * 1.1f);
            ChangeHP((int)(-totalFinalDamage));
            Damaged();
            Animator.SetTrigger(AnimationHash.DamagedParameterHash);
            DamageTextSpawner.Show((int)(totalFinalDamage), worldPos);                                    // 데미지 인디케이터 스폰

        }
        Debug.Log(finalDamage);
        //TODO: 피해입을때 효과,소리
    }


    void Damaged()
    {
        CancelInvoke(nameof(DamagedEnd));
        Sprite.color = Color.red;
        Invoke(nameof(DamagedEnd), 0.1f);
    }


    void DamagedEnd()
    {
        Sprite.color = Color.white;
    }

    /// <summary>
    /// 보스에게 디버프를 적용하는 메서드
    /// </summary>
    /// <param name="type">적용할 디버프 종류</param>
    /// <param name="duration">디버프 지속 시간</param>
    /// <param name="onApply">적용 시 실행할 행동</param>
    /// <param name="onExpire">디버프 끝날 시 실행할 행동</param>
    public void ApplyDebuff(DebuffType type, float duration, Action onApply = null, Action onExpire = null)
    {
        // 이미 존재하는 디버프만 시간 갱신
        if (ActiveDebuffs.ContainsKey(type))
        {
            ActiveDebuffs[type].StartTime = Time.time;
            ActiveDebuffs[type].Duration = duration;
            return;
        }

        // 디버프 인터페이스 넣어줌
        IDebuff debuffeffect = DebuffEffectFactory.Create(type);

        // 새 디버프 데이터 생성 및 등록
        var debuff = new DebuffData
        {
            Duration = duration,
            StartTime = Time.time,
            OnApply = onApply,
            OnExpire = onExpire,
            debuff = debuffeffect,
        };
        ActiveDebuffs[type] = debuff;

        // 디버프 시작할 때 로직 실행
        debuff.OnApply?.Invoke();

        // 디버프 자동으로 끝내기
        StartCoroutine(RemoveDebuffAfterTime(type, duration));
    }

    /// <summary>
    /// 디버프 끝날 때 자동으로 해제
    /// </summary>
    /// <param name="type">자동으로 해제시킬 디버프 타입</param>
    /// <param name="duration">디버프 지속 시간</param>
    /// <returns></returns>
    private IEnumerator RemoveDebuffAfterTime(DebuffType type, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (ActiveDebuffs.ContainsKey(type))
        {
            ActiveDebuffs[type].OnExpire?.Invoke();
            ActiveDebuffs.Remove(type);
        }
    }

    /// <summary>
    /// 특정 디버프가 지금 적용중인지 확인하는 메서드
    /// </summary>
    /// <param name="type">확인할 디버프 타입</param>
    /// <returns></returns>
    public bool HasDebuff(DebuffType type)
    {
        return ActiveDebuffs.ContainsKey(type);
    }

    /// <summary>
    /// 디버프 데이터 반환
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public DebuffData GetDebuffData(DebuffType type)
    {
        return ActiveDebuffs.TryGetValue(type, out var data) ? data : null;
    }
}
