using System;
using UniRx;
using UnityEngine;


/// <summary>
/// Skill을 만들기 위해 상속해야하는 기본 클래스
/// </summary>
public class Skill : ScriptableObject
{
    // 스킬 사용 메서드
    public virtual void UseSkill()
    {
        PlaySkillEffect();
    }
    // 모든 스킬에 공통으로 적용되는 플레이어 변수
    public Player player { get; set; }
    // 해당 스킬이 사용하고 있는 스킬 슬롯 enum 변수
    public SkillSlotKey slotKey { get; set; }
    public keyAction KeyAction { get; set; }
    // 해당 스킬이 어떤 키와 연결 되고 있는지 알려주는 문자열 변수

    [field: Header("스킬 이름")]
    [field: SerializeField]public string SkillName { get; private set; } = "스킬 이름";

    [field: Header("스킬 설명")]
    [field: SerializeField] public string SkillDesription { get; private set; } = "스킬 설명";

    [field: Header("스킬 타입")]
    [field: SerializeField] public SkillCategory SkillCategory { get; private set; } = SkillCategory.None;

    [field: Header("스킬 아이콘")]
    [field: SerializeField] public Sprite SkillIcon {get; private set; } // 스킬 아이콘

    [field: Header("스킬 사운드 설정")]
    [field: SerializeField] public EAudioClip EAudioClip { get; private set; }
    public bool CanUse { get; set; } = true; // 스킬 사용 가능 여부

    [field: Header("스킬 사용 중 움직임 가능 여부")]
    [field: SerializeField] public bool CanMove { get; private set; } = true; // 스킬 사용 중 움직임 가능 여부

    [field: Header("스킬 초기화할 쿨타임")]
    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; private set; } // 기본 쿨타임

        = new ReactiveProperty<float>(10f);
    [field: Header("스킬 현재 쿨타임(실시간)")]
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; private set; } // 현재 쿨타임

        = new ReactiveProperty<float>(0f);
    [field: Header("스킬을 사용하기 위해 연결할 State")]
    [field: SerializeField] public ApplyState ApplyState { get; private set; } = ApplyState.IdleState;// 연결해서 작동시킬 State 설정

    [field: Header("State에 진입할 때 실행할 Animation Enum")]
    [field: SerializeField] public AnimationState SkillEnterState { get; private set; } = AnimationState.Idle1;

    [field: Header("스킬을 사용할 때 실행할 Animation Enum")]
    [field: SerializeField] public AnimationState SkillUseState { get; private set; } = AnimationState.Idle1;

    [field: Header("Animation Sprite 1장이 교체되는 딜레이 시간")]
    [field: SerializeField] public float AnimationChangeDelayTime { get; set; } = 0.6f;

    [field: Header("이 스킬로 타격 시 쿨타임 감소 스킬에 영향을 줄 지에 대한 여부")]
    [field: SerializeField] public bool IsConnectSkillCoolDown { get; private set; } = false;

    //이 스킬이 적중할 때마다 실행하고 싶은 Action들을 저장
    [field: SerializeField] public Action AttackAction { get; set; } = null;
    [field: Header("스킬을 사용하기 전 효과를 줄 스킬 이펙트 애니메이터")]
    [field: SerializeField] public ESkillStartClipName SkillEffectsClipName { get; set; }

    [field: Header("스킬 이펙트 지속시간")]
    [field: SerializeField] public float SkillEffectsDuration { get; set; } = 1.0f;

    [field: Header("원하는 히트 이펙트 타입")]
    [field: SerializeField] public EHitEffectType HitEffectType { get; set; }

    [field: Header("스킬 레벨")] 
    public ReactiveProperty<int> Level = new ReactiveProperty<int>(1); // 스킬 레벨

    [field: Header("스킬 레벨별 배율")]
    [field: SerializeField] public float Magnification { get; private set; } = 0.1f; // 배율
    [field: Header("이 스킬을 업그레이드 할 때 지속시간이 증가되나요?")]
    [field: SerializeField] public bool isDurationUp { get; private set; } = false;

    // 플레이어 초기화
    public virtual void Init() { }

    public virtual void SkillUpgrade()
    {
        Level.Value++;
    }

    // 플레이어 방향 반환
    public float PlayerFrontXNormalized()
    {
        float x = player.IsFlipX ? -1f : 1f;
        return x;
    }

    // 플레이어 위치 반환
    public Vector3 PlayerPosition()
    {
        Vector3 playerPosition = player.transform.position;
        return playerPosition;
    }

    // 스킬 쿨타임 줄이기
    public void ReduceCooldown(float amount)
    {
        if (!CanUse)
        {
            CurCoolTime.Value = Mathf.Max(CurCoolTime.Value - amount, 0);
            if (CurCoolTime.Value == 0) CanUse = true;
        }
    }

    // 스킬 이펙트 재생
    public void PlaySkillEffect()
    {
        if (SkillEffectsClipName == ESkillStartClipName.None) return; 
        
        BasePoolable effectObj = PoolManager.Instance.Get(typeof(SkillEffectController));
        SkillEffectController effect = effectObj as SkillEffectController;

        // 이제 이펙트 설정해주기
        effect.Rpc_Init((int)SkillEffectsClipName, SkillEffectsDuration, player.transform.position, Vector3.one);
    }
}
