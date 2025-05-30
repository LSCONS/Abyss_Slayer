using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using static FoxClone;
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
    [field: SerializeField] public PlayerCheckGround playerCheckGround { get; private set; }
    [field: SerializeField] public BoxCollider2D PlayerGroundCollider { get; private set; }
    [field: SerializeField] public BoxCollider2D PlayerMeleeCollider { get; private set; }
    [field: SerializeField] public SpriteChange PlayerSpriteChange { get; private set; }
    [field: SerializeField] public GameObject LocalPlayerTargetObject { get; private set; }
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public PlayerData PlayerData { get; private set; }
    [field: Header("스킬 관련")]
    public Dictionary<SkillSlotKey, Skill> DictSlotKeyToSkill { get; private set; } = new(); // 스킬 연결용 딕셔너리
    public Dictionary<EBuffType, BuffSkill> DictBuffTypeToBuffSkill { get; private set; } = new();
    public ReactiveProperty<int> Hp { get; set; } = new();
    public ReactiveProperty<int> MaxHp { get; set; } = new();
    public ReactiveProperty<float> DamageValue { get; set; } = new(1);
    [HideInInspector] public float BaseDamage { get; private set; } = 1;
    [HideInInspector] public int BaseMaxHp { get; private set; }
    [HideInInspector] public int HpStatLevel { get; set; } = 1;
    [HideInInspector] public int DamageStatLevel { get; set; } = 1;
    public Coroutine HoldSkillCoroutine { get; private set; }
    public Action HoldSkillCoroutineStopAction { get; private set; }
    public NetworkData NetworkData { get; set; }
    public bool IsFlip
        => PlayerSpriteChange.WeaponBottom.flipX;
    [Networked] public float ArmorAmount { get; set; } = 1.0f;                   // 방어력 계수
    [Networked] public bool Invincibility { get; set; } = false;
    [Networked] public PlayerRef PlayerRef { get; set; }
    [Networked] public int PlayerStateIndex { get; set; } = -1;
    [Networked] public bool IsFlipX { get; set; } = false;
    [Networked] public Vector2 PlayerPosition { get; set; }
    public bool IsThisRunner => PlayerRef == Runner.LocalPlayer;

    public ReactiveProperty<int> StatPoint { get; set; } = new(1);
    public ReactiveProperty<int> SkillPoint { get; set; } = new(1);
    public NetworkInputData NetworkInput;


    public override async void Spawned()
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
        PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState, true);
        transform.position = PlayerPosition;
        StatPoint.Value = GameValueManager.Instance.InitStatusPointValue;
        SkillPoint.Value = GameValueManager.Instance.InitSkillPointValue;
        if (Runner.LocalPlayer == PlayerRef)
        {
            await ServerManager.Instance.WaitForPlayerState();
            ServerManager.Instance.UIPlayerState.UIHealthBar.ConnectPlayerObject(this);
            LocalPlayerTargetObject.SetActive(true);
        }
        else
        {
            LocalPlayerTargetObject.SetActive(false);
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
        if (!(Runner.IsServer)
            && PlayerStateMachine.DictIntToState[PlayerStateIndex] != PlayerStateMachine.currentState)
        {
            PlayerStateMachine.ChangeState(PlayerStateMachine.DictIntToState[PlayerStateIndex]);
        }

        SkillCoolTimeCompute();
        BuffDurationCompute();
        if (Runner.IsServer)
        {
            ComputeHealingTime();
        }

        PlayerStateMachine.Update();

        if (IsFlip != IsFlipX)
        {
            PlayerSpriteChange.SetFlipxSpriteRenderer(IsFlipX);
            IsFlipX = IsFlip;
        }

        if (Runner.IsServer) return;

        Vector2 target = PlayerPosition;
        Vector2 current = transform.position;

        if (current != target)
        {
            float t = GameValueManager.Instance.MoveSmoothObjectPositionForClientValue * Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(current, target, t);
            transform.position = newPos;
        }
    }


    /// <summary>
    /// 플레이어의 기본 체력 재생을 검사하는 메서드
    /// </summary>
    private void ComputeHealingTime()
    {
        if (MaxHp.Value == Hp.Value || PlayerData.PlayerStatusData.IsDead) return;
        PlayerStatusData data = PlayerData.PlayerStatusData;
        data.HealingCurTime -= Time.deltaTime;
        if (data.HealingCurTime <= 0)
        {
            Rpc_HealPlayerHP(data.HealingHealth);
            data.HealingCurTime = data.HealingDelay;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput<NetworkInputData>(out NetworkInput))
            PlayerStateMachine.FixedUpdate();
    }


    public async Task PlayerPositionReset(Vector2 position)
    {
        Rpc_SetAcitvePlayer(true);
        playerRigidbody.gravityScale = 0f;
        while ((Vector2)transform.position != position)
        {
            PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState, true);
            PlayerStateIndex = PlayerStateMachine.GetIntDictStateToInit(PlayerStateMachine.IdleState);
            playerRigidbody.velocity = Vector2.zero;
            PlayerPosition = position;
            transform.position = position;
            PlayerData.PlayerStatusData.IsDead = false;
            await Task.Delay(50);
        }
        return;
    }


    /// <summary>
    /// 코루틴과 Action을 등록시키고 실행시키는 메서드
    /// </summary>
    /// <param name="skill">실행 시킬 코루틴</param>
    /// <param name="action">종료할 때 실행시킬 Action</param>
    public void StartHoldSkillCoroutine(IEnumerator skill, Action action)
    {
        ;
        StopHoldSkillActionCoroutine();
        HoldSkillCoroutine = StartCoroutine(skill);
        HoldSkillCoroutineStopAction = action;
    }


    /// <summary>
    /// 등록해둔 Action을 실행시키며 코루틴을 종료시키는 메서드
    /// </summary>
    public void StopHoldSkillActionCoroutine()
    {
        ;
        if (HoldSkillCoroutine != null)
        {
            StopCoroutine(HoldSkillCoroutine);
            HoldSkillCoroutineStopAction?.Invoke();
            HoldSkillCoroutineStopAction = null;
            HoldSkillCoroutine = null;
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetAcitvePlayer(bool isActive)
    {
        gameObject.SetActive(isActive);
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


    /// <summary>
    /// 실행 중인 버프가 있을 경우 자동으로 카운트 해주는 메서드
    /// </summary>
    private void BuffDurationCompute()
    {
        foreach (var value in DictBuffTypeToBuffSkill.Values)
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
        foreach (var value in DictSlotKeyToSkill.Values)
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
        playerCheckGround.Init(this);
        // 선택된 클래스 정보 가져옴
        CharacterClass playerCharacterClass = NetworkData.Class;

        // 스프라이트 기본으로 init
        PlayerSpriteChange.Init(playerCharacterClass, NetworkData.HairStyleKey, NetworkData.FaceKey, NetworkData.SkinKey);

        // 플레이어 기본 데이터 로드 및 복사
        PlayerData = DataManager.Instance.DictClassToPlayerData[playerCharacterClass];
        PlayerData = Instantiate(PlayerData);

        Hp.Value = PlayerData.PlayerStatusData.HP_Cur;
        MaxHp.Value = PlayerData.PlayerStatusData.HP_Max;

        BaseMaxHp = MaxHp.Value;
        BaseDamage = PlayerData.PlayerStatusData.Damage_Base;

        //TODO: 이 데이터 언젠가 바꿔야함
        CharacterSkillSet skillSet = DataManager.Instance.DictClassToSkillSet[NetworkData.Class];
        skillSet = Instantiate(skillSet);
        skillSet.InstantiateSkillData(this);

        DictSlotKeyToSkill = new();
        foreach (CharacterSkillSlot slot in skillSet.skillSlots)
        {
            if (slot.skill != null)
            {
                DictSlotKeyToSkill[slot.key] = slot.skill;
            }
        }

        foreach (Skill skill in DictSlotKeyToSkill.Values)
        {
            skill.Init();
        }
    }


    /// <summary>
    /// 스킬의 쿨타임을 돌릴 메서드
    /// </summary>
    /// <param name="slotKey">쿨타임 돌릴 스킬 키</param>
    public void SkillCoolTimeUpdate(SkillSlotKey slotKey)
    {
        if (DictSlotKeyToSkill.ContainsKey(slotKey))
        {
            Skill skill = DictSlotKeyToSkill[slotKey];
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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Damage(int value, float attackPosX = -1000)
    {
        if (Hp.Value == 0 || Invincibility) return;
        value = (int)(value * PlayerData.PlayerStatusData.PlayerOnDamageLevelMultiple);
        float reduceDamage = value * ArmorAmount;
        int finalDamage = (int)MathF.Ceiling(reduceDamage);
        Damaged();
        Hp.Value = Hp.Value.PlusAndIntClamp(-finalDamage, MaxHp.Value);
        if (Hp.Value == 0)
        {
            PlayerDie();
        }
    }

    void Damaged()
    {
        CancelInvoke(nameof(DamagedEnd));
        PlayerSpriteChange.SetSpriteColor(Color.red);
        Invoke(nameof(DamagedEnd), GameValueManager.Instance.OnDamagePlayerColorDuration);
    }


    void DamagedEnd()
    {
        PlayerSpriteChange.SetSpriteColor(Color.white);
    }


    /// <summary>
    /// 플레이어 체력 회복 메서드
    /// </summary>
    /// <param name="value">변환을 줄 값. +를 넣어야 체력이 회복.</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_HealPlayerHP(int value)
    {
        Hp.Value = Hp.Value.PlusAndIntClamp(value, MaxHp.Value);
    }


    /// <summary>
    /// 플레이어 사망 관련 메서드
    /// </summary>
    public void PlayerDie()
    {
        PlayerStateMachine.ChangeState(PlayerStateMachine.DieState, true);

        // 게임오버 애널리틱스 전송
        PlayerData.PlayerStatusData.IsDead = true;
        int deadPlayerCount = 0;
        foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
        {
            if (player.PlayerData.PlayerStatusData.IsDead)
            {
                deadPlayerCount++;
            }
        }

        if (Runner.IsServer && deadPlayerCount == ServerManager.Instance.DictRefToPlayer.Values.Count)
        {
            StartCoroutine(PlayerDieCoroutine());
        }
    }

    /// <summary>
    /// 플레이어가 사망하면 특정 시간 이후 실행할 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDieCoroutine()
    {
        yield return new WaitForSeconds(3);

        if (GameValueManager.Instance.EGameLevel == EGameLevel.Easy)
        {
            ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.RestScene);
        }
        else
        {
            ServerManager.Instance.ExitRoom();
        }
    }

    /// <summary>
    /// 버프 활성화 메서드
    /// </summary>
    /// <param name="value">활성화 여부</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetBuff(int slotKeyInt)
    {
        Skill skill = DictSlotKeyToSkill[(SkillSlotKey)slotKeyInt];
        BuffSkill buffSkill = skill as BuffSkill;
        if (buffSkill != null)
        {
            buffSkill.CurBuffDuration.Value = buffSkill.MaxBuffDuration.Value;
            buffSkill.IsApply = true;
            DictBuffTypeToBuffSkill[buffSkill.Type] = buffSkill;
        }
    }


    /// <summary>
    /// 플레이어가 X좌표로 움직이는 방향을 계산하고 바꿔주는 메서드
    /// </summary>
    /// <param name="nowMoveX">움직이고 있는 X좌표값의 크기</param>
    public void FlipRenderer(float nowMoveX)
    {
        if (nowMoveX == 0 || IsFlipX == nowMoveX < 0) return;
        SpriteFlipXSynchro(nowMoveX < 0);
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

        //IdleState로 변환
        PlayerStateMachine.ChangeState(PlayerStateMachine.IdleState);
        //체력 Max로 변환
        Hp.Value = MaxHp.Value;
        PlayerData.PlayerStatusData.IsDead = false;
        //모든 스킬 쿨타임 0으로 변환
        //모든 버프 스킬 유지시간 0으로 변환
        foreach (Skill skill in DictSlotKeyToSkill.Values)
        {
            skill.CurCoolTime.Value = 0;
            BuffSkill buffSkill = skill as BuffSkill;
            if (buffSkill != null)
            {
                buffSkill.CurBuffDuration.Value = 0;
            }
        }
    }


    /// <summary>
    /// 플레이어의 상태를 공유하는 메서드
    /// </summary>
    /// <param name="stateIndex"></param>
    public void ChagneState(int stateIndex)
    {
        PlayerStateIndex = stateIndex;
    }


    /// <summary>
    /// 플레이어의 포지션을 공유하는 메서드
    /// </summary>
    /// <param name="playerPosition"></param>
    public void PlayerPositionSynchro(Vector2 playerPosition)
    {
        PlayerPosition = playerPosition;
    }


    /// <summary>
    /// 플레이어의 좌우 뒤집힌 상태를 공유하는 메서드
    /// </summary>
    /// <param name="flipX"></param>
    public void SpriteFlipXSynchro(bool flipX)
    {
        IsFlipX = flipX;
    }

    /// <summary>
    /// 스탯 업그레이드 적용하는 메서드
    /// </summary>
    /// <param name="hpLevel"></param>
    /// <param name="dmgLevel"></param>
    /// <param name="amount"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ApplyStatUpgrade(int hpLevel, int dmgLevel, float amount)
    {
        HpStatLevel = hpLevel;
        DamageStatLevel = dmgLevel;

        MaxHp.Value = Mathf.RoundToInt(BaseMaxHp * (1f + amount * (hpLevel - 1)));
        DamageValue.Value = BaseDamage * (1f + amount * (dmgLevel - 1));
    }




    /// <summary>
    /// 스킬 포인트를 올리는 메서드
    /// </summary>
    /// <param name="count">올릴 스킬 포인트 양</param>
    public void AddSkillPoint(int count)
    {
        SkillPoint.Value += count;
        ServerManager.Instance.UISkillUpgradeStore?.Init();
    }


    /// <summary>
    /// 스텟 포인트를 올리는 메서드
    /// </summary>
    /// <param name="count">올릴 스킬 포인트 양</param>
    public void AddStatusPoint(int count)
    {
        StatPoint.Value += count;
        ServerManager.Instance.UIStatUpgradeStore?.Init();
    }
}
