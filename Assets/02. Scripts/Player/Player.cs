using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UniRx;
using System.Threading.Tasks;
using UnityEngine.UI;
public enum CharacterClass
{
    Rogue,
    Healer,
    Mage,
    MagicalBlader,
    Tanker,

    Count
}

public class Player : MonoBehaviour, IHasHealth
{
    public CharacterClass playerCharacterClass;
    public PlayerInput input { get; private set; }
    public Rigidbody2D playerRigidbody;
    public PlayerCheckGround playerCheckGround;
    public CinemachineVirtualCamera mainCamera;//TODO: 나중에 초기화 필요
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public BoxCollider2D PlayerGroundCollider {  get; private set; }
    public BoxCollider2D PlayerMeleeCollider { get; private set; }
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    [field: Header("스킬 관련")]
    public Dictionary<SkillSlotKey, Skill> EquippedSkills { get; private set; } = new(); // 스킬 연결용 딕셔너리
    public Dictionary<BuffType, BuffSkill> BuffDuration { get; private set; } = new();
    public ReactiveProperty<int> Hp { get; set; } = new();
    public ReactiveProperty<int> MaxHp { get; set; } = new();
    public ReactiveProperty<float> DamageValue { get; set; } = new(1);
    public float ArmorAmount { get; set; } = 1.0f;                   // 방어력 계수

    public event Action<Skill> OnSkillHit;   // 스킬 적중할 때, 그 스킬 알려주는 이벤트
    public bool IsFlipX { get; private set; } = false;
    public SpriteChange PlayerSpriteChange { get; private set; }
    public Coroutine HoldSkillCoroutine { get; private set; }
    public Action HoldSkillCoroutineStopAction { get; private set; }

    public int HpStatLevel = 1;
    public int DamageStatLevel = 1;

    public ReactiveProperty <int> StatPoint { get; set; } = new(10);
    public ReactiveProperty <int> SkillPoint { get; set; } = new(10);

    public void StartHoldSkillCoroutine(IEnumerator skill, Action action)
    {
        StopHoldSkillActionCoroutine();
        HoldSkillCoroutine = StartCoroutine(skill);
        HoldSkillCoroutineStopAction = action;
    }

    public void StopHoldSkillActionCoroutine()
    {
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutineStopAction?.Invoke();
            HoldSkillCoroutineStopAction = null;
            HoldSkillCoroutine = null;
        }
    }

    public void StopHoldSkillNoneCoroutine()
    {
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutineStopAction = null;
            HoldSkillCoroutine = null;
        }
    }

    private void Awake()
    {
        InitComponent();
        InitPlayerData();
        PlayerStateMachine = new PlayerStateMachine(this);
        playerCheckGround.playerTriggerOff += PlayerColliderTriggerOff;
    }


    private void Start()
    {
        PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState);
    }

    private void Update()
    {
        PlayerStateMachine.Update();
        SkillCoolTimeCompute();
        BuffDurationCompute();
    }

    private void FixedUpdate()
    {
        PlayerStateMachine.FixedUpdate();
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
        foreach (var value in EquippedSkills.Values)
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

        // 선택된 클래스 정보 가져옴
        playerCharacterClass = PlayerManager.Instance.GetSelectedClass();
        CharacterSkillSet skillSet = null;

        // 스프라이트 기본으로 init
        PlayerSpriteChange.Init(playerCharacterClass);

        // 커스터마이징 된 스프라이트를 적용
        var info = PlayerManager.Instance.PlayerCustomizationInfo;
        var data = DataManager.Instance;

        SetAllAnimationStates(PlayerSpriteChange.Skin, data.DictIntToDictStateToSkinColorSprite, info.skinId);
        SetAllAnimationStates(PlayerSpriteChange.Face, data.DictIntToDictStateToFaceColorSprite, info.faceId);
        SetAllAnimationStates(PlayerSpriteChange.HairTop, data.DictIntToDictStateToHairStyleTopSprite, info.hairId);
        SetAllAnimationStates(PlayerSpriteChange.HairBottom, data.DictIntToDictStateToHairStyleBottomSprite, info.hairId);

        // 플레이어 기본 데이터 로드 및 복사
        PlayerData = Resources.Load<PlayerData>("Player/PlayerData/PlayerData");
        PlayerData = Instantiate(PlayerData);
        Hp.Value = PlayerData.PlayerStatusData.HP_Cur;
        MaxHp.Value = PlayerData.PlayerStatusData.HP_Max;
        //TODO: 여기서부터 임시 코드
        switch (playerCharacterClass)
        {
            case CharacterClass.Rogue:
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/RogueSkillSet");
                break;
            case CharacterClass.Healer:
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/HealerSkillSet");
                break;
            case CharacterClass.Mage:
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/MageSkillSet");
                break;
            case CharacterClass.MagicalBlader:
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/MagicalBladerSkillSet");
                break;
            case CharacterClass.Tanker:
                skillSet = Resources.Load<CharacterSkillSet>("Player/PlayerSkillSet/TankerSkillSet");
                break;
        }

        // 스킬 등록
        skillSet = Instantiate(skillSet);
        skillSet.InstantiateSkillData(this);
        EquippedSkills = new();
        foreach (CharacterSkillSlot slot in skillSet.skillSlots)
        {
            if (slot.skill != null)
            {
                EquippedSkills[slot.key] = slot.skill;
            }
        }
        foreach (Skill skill in EquippedSkills.Values)
        {
            skill.Init();
        }
    }
    private void SetAllAnimationStates(SpriteRenderer target, Dictionary<int, Dictionary<AnimationState, Sprite[]>> sourceDict, int id)
    {
        if (!PlayerSpriteChange.DictAnimationState.ContainsKey(target))
            PlayerSpriteChange.DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

        if (sourceDict.TryGetValue(id, out var animDict))
        {
            foreach (var pair in animDict)
            {
                PlayerSpriteChange.DictAnimationState[target][pair.Key] = pair.Value;
            }
        }
    }

    /// <summary>
    /// 컴포넌트를 초기화하는 메서드
    /// </summary>
    private void InitComponent()
    {
        input = GetComponent<PlayerInput>();
        input.InitDictionary();
        playerRigidbody = GetComponent<Rigidbody2D>();
        PlayerSpriteChange = GetComponentInChildren<SpriteChange>();
        playerCheckGround = transform.GetComponentForTransformFindName<PlayerCheckGround>("Collider_GroundCheck");
        PlayerGroundCollider = transform.GetComponentForTransformFindName<BoxCollider2D>("Collider_GroundCheck");
        PlayerMeleeCollider = transform.GetComponentForTransformFindName<BoxCollider2D>("Collider_MeleeDamageCheck");
    }


    /// <summary>
    /// 스킬의 쿨타임을 돌릴 메서드
    /// </summary>
    /// <param name="slotKey">쿨타임 돌릴 스킬 키</param>
    public void SkillCoolTimeUpdate(SkillSlotKey slotKey)
    {
        if (EquippedSkills.ContainsKey(slotKey))
        {
            Skill skill = EquippedSkills[slotKey];
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
    public void Damage(int value, float attackPosX = -1000)
    {
        float reduceDamage = value * ArmorAmount;
        int finalDamage = (int)MathF.Ceiling(reduceDamage);

        Hp.Value = Hp.Value.PlusAndIntClamp(-finalDamage, MaxHp.Value);
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
        StartCoroutine(PlayerDieCo());
    }

    private IEnumerator PlayerDieCo()
    {
        yield return new WaitForSeconds(3);
        GameFlowManager.Instance.ChangeState(EGameState.Lobby);

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

    /// <summary>
    ///  스킬 데미지 들어갈 때 호출 hitskill 전달해줌
    /// </summary>
    /// <param name="hitSkill"></param>
    public void RaiseSkillHit(Skill hitSkill)
    {
        OnSkillHit?.Invoke(hitSkill);
    }

    /// <summary>
    /// 플레이어가 X좌표로 움직이는 방향을 계산하고 바꿔주는 메서드
    /// </summary>
    /// <param name="nowMoveX">움직이고 있는 X좌표값의 크기</param>
    public void FlipRenderer(float nowMoveX)
    {
        if (nowMoveX > 0)
        {
            IsFlipX = false;
        }
        else if (nowMoveX < 0)
        {
            IsFlipX = true;
        }
        PlayerSpriteChange.SetFlipxSpriteRenderer(IsFlipX);
    }

}
