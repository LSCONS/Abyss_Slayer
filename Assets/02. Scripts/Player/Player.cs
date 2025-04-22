using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UniRx;
using Fusion;
using Fusion.Addons.Physics;
public enum CharacterClass
{
    Archer,
    Healer,
    Mage,
    MagicalBlader,
    Tanker
}

public class Player : NetworkBehaviour, IHasHealth
{
    public CharacterClass playerCharacterClass;
    public NetworkInputData Input { get; private set; }
    public Rigidbody2D PlayerRigidbody {  get; private set; }
    public NetworkRigidbody2D PlayerNetworkRigidbody { get; private set; }
    public PlayerCheckGround PlayerCheckGround { get; private set; }
    public CinemachineVirtualCamera MainCamera { get; private set; }    //TODO: 나중에 초기화 필요
    public Animator PlayerAnimator { get; private set; }                //TODO: 나중에 초기화 필요
    public PlayerAnimationData PlayerAnimationData { get; private set; }
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public BoxCollider2D PlayerGroundCollider {  get; private set; }
    public BoxCollider2D PlayerMeleeCollider { get; private set; }
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    [Networked, OnChangedRender(nameof(SetSpriteFlipX))]
    public NetworkBool IsFlipX { get; set; } = false;

    [Header("스킬 관련")]
    public Dictionary<SkillSlotKey, Skill> equippedSkills = new(); // 스킬 연결용 딕셔너리
    public CharacterSkillSet skillSet; // 스킬셋 데이터
    public IStopCoroutine SkillTrigger { get; private set; }

    public Dictionary<BuffType, BuffSkill> BuffDuration { get; private set; } = new();

    public ReactiveProperty<int> Hp { get; set; } = new();

    public ReactiveProperty<int> MaxHp { get; set; } = new();

    public Action<BoxCollider2D, float> OnMeleeAttack;  // 근접 공격 콜라이더 ON/OFF 액션

    public event Action<Skill> OnSkillHit;   // 스킬 적중할 때, 그 스킬 알려주는 이벤트

    private void Awake()
    {
        InitComponent();
        InitPlayerData();
        InitSkillData();
        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerCheckGround.playerTriggerOff += PlayerColliderTriggerOff;
        OnMeleeAttack += (collider, duration) => StartCoroutine(EnableMeleeCollider(collider, duration));
    }


    private void Start()
    {
        PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState);
    }

    private void FixedUpdate()
    {
        PlayerStateMachine.FixedUpdate();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data)) Input = data;
        PlayerStateMachine.FixedUpdateNetwork();
    } 

    private void Update()
    {
        PlayerStateMachine.Update();
        SkillCoolTimeCompute();
        BuffDurationCompute();
    }


    private void BuffDurationCompute()
    {
        foreach (var value in BuffDuration.Values)
        {
            if (value.IsApply)
            {
                value.CurBuffDuration.Value -= Time.deltaTime;
                if (value.CurBuffDuration.Value <= 0)
                {
                    value.CurBuffDuration.Value = 0;
                    value.IsApply = false;
                }
            }

        }
    }


    /// <summary>
    /// 각 스킬이 사용 가능한지 확인하고 불가능하다면 쿨타임을 계산하는 메서드
    /// </summary>
    private void SkillCoolTimeCompute()
    {
        foreach (var value in equippedSkills.Values)
        {
            if (!(value.CanUse))
            {
                value.CurCoolTime.Value -= Time.deltaTime;
                if (value.CurCoolTime.Value <= 0)
                {
                    value.CurCoolTime.Value = 0;
                    value.CanUse = true;
                }
            }
        }
    }


    /// <summary>
    /// 플레이어 데이터를 초기화하는 메서드
    /// </summary>
    private void InitPlayerData()
    {
        //TODO: 임시 플레이어 데이터 복사 나중에 개선 필요
        PlayerAnimationData = PlayerManager.Instance.PlayerAnimationData;
        PlayerData = Resources.Load<PlayerData>("Player/PlayerData/PlayerData");
        PlayerData = Instantiate(PlayerData);
        Hp.Value = PlayerData.PlayerStatusData.HP_Cur;
        MaxHp.Value = PlayerData.PlayerStatusData.HP_Max;
        //TODO: 여기서부터 임시 코드
        switch (playerCharacterClass)
        {
            case CharacterClass.Archer:
                SkillTrigger = PlayerAnimator.gameObject.AddComponent<ArcherSkillAnimationTrigger>() as IStopCoroutine;
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/ArcherSkillSet");
                break;
            case CharacterClass.Healer:
                SkillTrigger = PlayerAnimator.gameObject.AddComponent<HealerSkillAnimationTrigger>() as IStopCoroutine;
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/HealerSkillSet");
                break;
            case CharacterClass.Mage:
                SkillTrigger = PlayerAnimator.gameObject.AddComponent<MageSkillAnimationTrigger>() as IStopCoroutine;
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/MageSkillSet");
                break;
            case CharacterClass.MagicalBlader:
                SkillTrigger = PlayerAnimator.gameObject.AddComponent<MagicalBladerSkillAnimationTrigger>() as IStopCoroutine;
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/MagicalBladerSkillSet");
                break;
            case CharacterClass.Tanker:
                SkillTrigger = PlayerAnimator.gameObject.AddComponent<TankerSkillAnimationTrigger>() as IStopCoroutine;
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/TankerSkillSet");
                break;
        }
    }


    /// <summary>
    /// 컴포넌트를 초기화하는 메서드
    /// </summary>
    private void InitComponent()
    {
        PlayerNetworkRigidbody = GetComponent<NetworkRigidbody2D>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
        PlayerGroundCollider = transform.GetComponentForTransformFindName<BoxCollider2D>("Collider_GroundCheck");
        PlayerMeleeCollider = transform.GetComponentForTransformFindName<BoxCollider2D>("Collider_MeleeDamageCheck");
        SpriteRenderer = transform.GetComponentForTransformFindName<SpriteRenderer>("Sprtie_Player");
        PlayerAnimator = transform.GetComponentForTransformFindName<Animator>("Sprtie_Player");
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    /// <summary>
    /// 스킬 데이터를 딕셔너리 형태로 초기화하는 메서드
    /// </summary>
    private void InitSkillData()
    {
        skillSet = Instantiate(skillSet);
        skillSet.InstantiateSkillData(this);
        equippedSkills = new();
        foreach (var slot in skillSet.skillSlots)
        {
            if (slot.skill != null)
            {
                equippedSkills[slot.key] = slot.skill;
            }
            else
            {
                Debug.LogWarning($"Skill in slot {slot.key} is null!");
            }
        }
        SkillTrigger.Player = this;
        SkillTrigger.SkillDictionary = equippedSkills;
    }


    /// <summary>
    /// 스킬의 쿨타임을 돌릴 메서드
    /// </summary>
    /// <param name="slotKey">쿨타임 돌릴 스킬 키</param>
    public void SkillCoolTimeUpdate(SkillSlotKey slotKey)
    {
        if (equippedSkills.ContainsKey(slotKey))
        {
            Skill skill = equippedSkills[slotKey];
            skill.CurCoolTime.Value = skill.MaxCoolTime.Value;
            skill.CanUse = false;
        }
    }


    /// <summary>
    /// 플레이어가 다운점프를 하고 빠져나온 뒤 isTrigger을 false로 되돌리는 메서드
    /// </summary>
    private void PlayerColliderTriggerOff()
    {
        PlayerGroundCollider.isTrigger = false;
    }


    /// <summary>
    /// 플레이어 체력 닳는 메서드
    /// </summary>
    /// <param name="value">변환을 줄 값. +를 넣어야 체력이 깎임.</param>
    public void OnDamagePlayerHP(int value)
    {
        Hp.Value = Hp.Value.PlusAndIntClamp(-value, MaxHp.Value);
        if (Hp.Value == 0)
        {
            PlayerDie();
        }
    }


    /// <summary>
    /// 플레이어 체력 회복 메서드
    /// </summary>
    /// <param name="value">변환을 줄 값. +를 넣어야 체력이 회복.</param>
    public void HealPlayerHP(int value)
    {
        Hp.Value = Hp.Value.PlusAndIntClamp(value, MaxHp.Value);
    }


    /// <summary>
    /// 플레이어 사망 관련 메서드
    /// </summary>
    public void PlayerDie()
    {
        PlayerStateMachine.ChangeState(PlayerStateMachine.DieState);
    }


    /// <summary>
    /// 버프 활성화 메서드
    /// </summary>
    /// <param name="value">활성화 여부</param>
    public void SetBuff(Skill skill)
    {
        BuffSkill buffSkill = skill as BuffSkill;
        if(buffSkill != null)
        {
            buffSkill.CurBuffDuration.Value = buffSkill.MaxBuffDuration.Value;
            buffSkill.IsApply = true;
            BuffDuration[buffSkill.Type] = buffSkill; 
        }
    }

    public IEnumerator EnableMeleeCollider(BoxCollider2D collider, float duration)
    {
        collider.enabled = true;
        yield return new WaitForSeconds(duration);
        collider.enabled = false;
    }

    /// <summary>
    ///  스킬 데미지 들어갈 때 호출 hitskill 전달해줌
    /// </summary>
    /// <param name="hitSkill"></param>
    public void RaiseSkillHit(Skill hitSkill)
    {
        OnSkillHit?.Invoke(hitSkill);
    }


    private void SetSpriteFlipX()
    {
        SpriteRenderer.flipX = IsFlipX;
    }

    public void SetFlipX(float moveX)
    {
        if(moveX < 0)
        {
            IsFlipX = true;
            return;
        }

        if(moveX > 0)
        {
            IsFlipX = false;
            return;
        }
    }
}
