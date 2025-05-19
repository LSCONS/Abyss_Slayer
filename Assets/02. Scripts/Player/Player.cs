using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
public enum CharacterClass
{
    Rogue,
    Healer,
    Mage,
    MagicalBlader,
    Tanker,
    Count
}


[SimulationBehaviour(
    Stages = SimulationStages.Forward
)]
public class Player : NetworkBehaviour, IHasHealth
{
    [field: SerializeField] public Rigidbody2D playerRigidbody { get; private set; }
    [field: SerializeField] public PlayerInput PlayerInput { get;private set; }
    [field: SerializeField] public PlayerCheckGround playerCheckGround { get; private set; }
    [field: SerializeField] public BoxCollider2D PlayerGroundCollider {  get; private set; }
    [field: SerializeField] public BoxCollider2D PlayerMeleeCollider { get; private set; }
    [field: SerializeField] public SpriteChange PlayerSpriteChange { get; private set; }
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public PlayerData PlayerData { get; private set; }
    [field: Header("스킬 관련")]
    public Dictionary<SkillSlotKey, Skill> EquippedSkills { get; private set; } = new(); // 스킬 연결용 딕셔너리
    public Dictionary<BuffType, BuffSkill> BuffDuration { get; private set; } = new();
    public ReactiveProperty<int> Hp { get; set; } = new();
    public ReactiveProperty<int> MaxHp { get; set; } = new();
    public ReactiveProperty<float> DamageValue { get; set; } = new(1);
    public float ArmorAmount { get; set; } = 1.0f;                   // 방어력 계수
    public Coroutine HoldSkillCoroutine { get; private set; }
    public Action HoldSkillCoroutineStopAction { get; private set; }
    public NetworkData NetworkData { get; set; }
    public bool IsFlip
        => PlayerSpriteChange.WeaponBottom.flipX;
    //public NetworkInputData PlayerInputData;
    [Networked] public PlayerRef PlayerRef { get; set; }
    [Networked] public int PlayerStateIndex { get; set; } = -1;
    [Networked] public bool IsFlipX { get; set; } = false;
    [Networked] public Vector2 PlayerPosition { get; set; }
    public int tempSmooth = 5;
    public bool IsThisRunner => PlayerRef == Runner.LocalPlayer;

    public int HpStatLevel = 1;
    public int DamageStatLevel = 1;

    public ReactiveProperty <int> StatPoint { get; set; } = new(10);
    public ReactiveProperty <int> SkillPoint { get; set; } = new(10);

    /// <summary>
    /// 코루틴과 Action을 등록시키고 실행시키는 메서드
    /// </summary>
    /// <param name="skill">실행 시킬 코루틴</param>
    /// <param name="action">종료할 때 실행시킬 Action</param>
    public void StartHoldSkillCoroutine(IEnumerator skill, Action action)
    {;
        StopHoldSkillActionCoroutine();
        HoldSkillCoroutine = StartCoroutine(skill);
        HoldSkillCoroutineStopAction = action;
    }

    /// <summary>
    /// 등록해둔 Action을 실행시키며 코루틴을 종료시키는 메서드
    /// </summary>
    public void StopHoldSkillActionCoroutine()
    {;
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutineStopAction?.Invoke();
            HoldSkillCoroutineStopAction = null;
            HoldSkillCoroutine = null;
        }
    }

    /// <summary>
    /// 등록해둔 Action을 무시하고 코루틴을 종료시키는 메서드
    /// </summary>
    public void StopHoldSkillNoneCoroutine()
    {
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutineStopAction = null;
            HoldSkillCoroutine = null;
        }
    }

    public override void Spawned()
    {
        gameObject.SetActive(false);
        base.Spawned();
        ServerManager.Instance.DictRefToPlayer[PlayerRef] = this;
        NetworkData = ServerManager.Instance.DictRefToNetData[PlayerRef];
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        InitPlayerData();
        playerCheckGround.playerTriggerOff += PlayerColliderTriggerOff;
        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState);
        transform.position = PlayerPosition;
        if(Runner.LocalPlayer != PlayerRef)
        {
            playerRigidbody.simulated = false;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        gameObject.SetActive(false);
        if (ServerManager.Instance.DictRefToPlayer[PlayerRef] != this)
        ServerManager.Instance.DictRefToPlayer.Remove(PlayerRef);
        base.Despawned(runner, hasState);
    }

    private void Update()
    {
        if (!(IsThisRunner)
            && PlayerStateMachine.DictIntToState[PlayerStateIndex] != PlayerStateMachine.currentState)
        {
            PlayerStateMachine.ChangeState(PlayerStateMachine.DictIntToState[PlayerStateIndex]);
        }

            PlayerStateMachine.Update();
            SkillCoolTimeCompute();
            BuffDurationCompute();

        if (IsFlip != IsFlipX)
        {
            PlayerSpriteChange.SetFlipxSpriteRenderer(IsFlipX);
            IsFlipX = IsFlip;
        }

        if (Runner.LocalPlayer == PlayerRef) return;
        // 네트워크로 받은 타깃 위치
        Vector2 target = PlayerPosition;

        // 현재 위치
        Vector2 current = transform.position;

        if (current != target)
        {
            // t = smoothingSpeed * Time.deltaTime
            // Time.deltaTime: 프레임 간격(초)
            float t = tempSmooth * Time.deltaTime;

            // 보간 적용 (t가 1이면 즉시, 0이면 전혀 이동 안 함)
            Vector2 newPos = Vector2.Lerp(current, target, t);

            transform.position = newPos;
        }
    }

    private void FixedUpdate()
    {
        PlayerStateMachine.FixedUpdate();
    }


    /// <summary>
    /// 실행 중인 버프가 있을 경우 자동으로 카운트 해주는 메서드
    /// </summary>
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
        CharacterClass playerCharacterClass = PlayerManager.Instance.selectedCharacterClass;
        CharacterSkillSet skillSet = null;

        // 스프라이트 기본으로 init
        PlayerSpriteChange.Init(playerCharacterClass, (NetworkData.HairStyleKey, NetworkData.HairColorKey), NetworkData.FaceColorKey, NetworkData.SkinColorKey);

        // 커스터마이징 된 스프라이트를 적용
        var info = PlayerManager.Instance.PlayerCustomizationInfo;
        var data = DataManager.Instance;

        // 플레이어 기본 데이터 로드 및 복사
        PlayerData = Resources.Load<PlayerData>("Player/PlayerData/PlayerData");
        PlayerData = Instantiate(PlayerData);

        Hp.Value = PlayerData.PlayerStatusData.HP_Cur;
        MaxHp.Value = PlayerData.PlayerStatusData.HP_Max;

        //TODO: 이 데이터 언젠가 바꿔야함
        skillSet = DataManager.Instance.DictClassToSkillSet[NetworkData.Class];
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
    private void SetAllAnimationStates(SpriteRenderer target, Dictionary<(int, int), Dictionary<AnimationState, Sprite[]>> sourceDict, (int, int) id)
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
        PlayerInput = GetComponent<PlayerInput>();
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
        
        // 게임오버 애널리틱스 전송
        int deadPlayerCount = 0;
        foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
        {
            if (player.Hp.Value <= 0)
            {
                deadPlayerCount++;
            }
        }

        StartCoroutine(PlayerDieCoroutine());
    }

    /// <summary>
    /// 플레이어가 사망하면 특정 시간 이후 실행할 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDieCoroutine()
    {
        yield return new WaitForSeconds(3);
        GameFlowManager.Instance.RpcServerSceneLoad(ESceneName.LobbyScene);

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
    /// 플레이어가 X좌표로 움직이는 방향을 계산하고 바꿔주는 메서드
    /// </summary>
    /// <param name="nowMoveX">움직이고 있는 X좌표값의 크기</param>
    public void FlipRenderer(float nowMoveX)
    {
        if (nowMoveX == 0 || IsFlipX == nowMoveX < 0) return;
        Rpc_SpriteFlipXSynchro(nowMoveX < 0);
    }


    /// <summary>
    /// 해당 플레이어의 모든 상태를 초기화하는 메서드
    /// </summary>
    public void ResetPlayerStatus()
    {
#if AllMethodDebug
        Debug.Log("ResetPlayerStatus");
#endif
        //비활성화
        gameObject.SetActive(false);

        // if(PlayerRef == Runner.LocalPlayer)
        // {
        //     //IdleState로 변환
        //     PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState);
        //     //체력 Max로 변환
        //     Hp.Value = MaxHp.Value;
        //     //모든 스킬 쿨타임 0으로 변환
        //     //모든 버프 스킬 유지시간 0으로 변환
        //     foreach (Skill skill in EquippedSkills.Values)
        //     {
        //         skill.CurCoolTime.Value = 0;
        //         BuffSkill buffSkill = skill as BuffSkill;
        //         if (buffSkill != null)
        //         {
        //             buffSkill.CurBuffDuration.Value = 0;
        //         }
        //     }
        //     //위치 값 0으로 초기화
        //     transform.position = Vector3.zero;
        //     Rpc_PlayerPositionSynchro(Vector2.zero);
        // }
    }


    /// <summary>
    /// 플레이어의 상태를 공유하는 메서드
    /// </summary>
    /// <param name="stateIndex"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ChagneState(int stateIndex)
    {
        PlayerStateIndex = stateIndex;
    }


    /// <summary>
    /// 플레이어의 포지션을 공유하는 메서드
    /// </summary>
    /// <param name="playerPosition"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_PlayerPositionSynchro(Vector2 playerPosition)
    {
        PlayerPosition = playerPosition;
    }


    /// <summary>
    /// 플레이어의 좌우 뒤집힌 상태를 공유하는 메서드
    /// </summary>
    /// <param name="flipX"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SpriteFlipXSynchro(bool flipX)
    {
        IsFlipX = flipX;
    }
}
