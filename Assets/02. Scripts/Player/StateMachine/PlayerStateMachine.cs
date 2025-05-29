using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerDeadState DieState { get; private set; }

    public Dictionary<SkillSlotKey, PlayerSkillEnterState> PlayerSkillEnterStateDict { get; private set; } = new();
    public Dictionary<SkillSlotKey, PlayerSkillUseState> PlayerSkillUseStateDict { get; private set; } = new();

    public StoppableAction SkipAttackAction = new();
    public StoppableAction EndAttackAction = new();

    public bool IsDash { get; set; } = false;       //플레이어가 현재 대시 상태인지 출력하는 변수
    public bool CanMove { get; set; } = true;
    public bool DidDownJump { get; set; } = false;  // 플레이어가 아랫점프 중인지 확인
    public bool WasJumpPressedLastFrame { get; set; } = false;
    public AnimatorStateInfo AnimatorInfo { get; set; }
    public bool IsCompareState { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public Dictionary<int, IPlayerState> DictIntToState { get; private set; } = new();
    public HashSet<IPlayerState> HashPlayerState { get; private set; }
    public Skill UseSkillData { get; set; }

    public PlayerStateMachine(Player player)
    {
        int num = 0;
        this.Player = player;

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);
        DieState = new PlayerDeadState(this);
        DictIntToState[num++] = IdleState;
        DictIntToState[num++] = WalkState;
        DictIntToState[num++] = JumpState;
        DictIntToState[num++] = FallState;
        DictIntToState[num++] = DieState;

        foreach (var key in player.DictSlotKeyToSkill.Keys)
        {
            PlayerSkillEnterStateDict[key] = new PlayerSkillEnterState(this, key);
            PlayerSkillUseStateDict[key] =  new PlayerSkillUseState(this, key);
            DictIntToState[num++] = PlayerSkillEnterStateDict[key];
            DictIntToState[num++] = PlayerSkillUseStateDict[key];
        }

        HashPlayerState = new HashSet<IPlayerState>(DictIntToState.Values);

        IdleState.Init();
        WalkState.Init();
        JumpState.Init(); 
        FallState.Init(); 
        DieState.Init();


        foreach (var key in player.DictSlotKeyToSkill.Keys)
        {
            PlayerSkillEnterStateDict[key].Init();
            PlayerSkillUseStateDict[key].Init();
        }

        SkipAttackAction.AddListener(ConnectJumpState);
        SkipAttackAction.AddListener(ConnectDashState);

        EndAttackAction.AddListener(ConnectFallState);
        EndAttackAction.AddListener(ConnectIdleState);
        EndAttackAction.AddListener(ConnectWalkState);

        MovementSpeed = Player.PlayerData.PlayerGroundData.BaseSpeed;
    }

    public void UseSkill()
    {
        UseSkillData?.UseSkill();
        UseSkillData = null;
    }


    public override void ChangeState(IPlayerState state, bool loading = false)
    {
        if (!(ServerManager.Instance.PlayerInput.IsConnectInput) && !(loading)) return;
        base.ChangeState(state);
        if (!(Player.Runner.IsServer)) return;
        Player.ChagneState(GetIntDictStateToInit(state));
    }

    /// <summary>
    /// SkillState에서 연결 가능한 MoveState를 찾고 연결하는 메서드
    /// </summary>
    /// <param name="state">연결하고 싶은 SkillState</param>
    public void ConnectSkillState(IPlayerState state, Skill skillData, System.Func<bool> isAction)
    {
        ApplyState applyState = skillData.ApplyState;

        if ((ApplyState.IdleState | applyState) == applyState)
            IdleState.MoveAction.AddListener(() => ConnectAttackAction(isAction(), state, skillData));

        if ((ApplyState.WalkState | applyState) == applyState)
            WalkState.MoveAction.AddListener(() => ConnectAttackAction(isAction(), state, skillData));

        if ((ApplyState.JumpState | applyState) == applyState)
            JumpState.MoveAction.AddListener(() => ConnectAttackAction(isAction(), state, skillData));

        if ((ApplyState.FallState | applyState) == applyState)
            FallState.MoveAction.AddListener(() => ConnectAttackAction(isAction(), state, skillData));
    }


    /// <summary>
    /// State에 있는 AttackAction과 연결되는 메서드
    /// </summary>
    /// <param name="isAction">입력 키 토글 여부</param>
    /// <param name="state">변환할 State</param>
    /// <param name="skillData">참고할 SkillData</param>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    private bool ConnectAttackAction(bool isAction, IPlayerState state, Skill skillData)
    {
        if (isAction && skillData.CanUse)
        {
            ChangeState(state);
            return true;
        }
        return false;
    }

    /// <summary>MoveDir.X의 값이 0인지 확인</summary>
    private bool IsZeroMoveDirX() => Player.NetworkInput.MoveDir.x == 0;

    /// <summary>MoveDir의 X값이 0인지 확인</summary>
    private bool IsZeroMoveDir() => Player.NetworkInput.MoveDir == Vector2.zero;

    /// <summary>점프가 가능한 상태인지 확인</summary>
    private bool IsCanJump() => Player.playerCheckGround.CanJump;

    /// <summary>isTrigger가 어떤 상태인지 확인</summary>
    private bool IsTrigger() => Player.PlayerGroundCollider.isTrigger;

    /// <summary>isTrigger을 true로 변경</summary>
    public void IsTriggerTrue() => Player.PlayerGroundCollider.isTrigger = true;

    /// <summary>velocity.y의 값이 0에 가까운지 확인</summary>
    private bool IsZeroVelocityY() => Mathf.Approximately(Player.playerRigidbody.velocity.y, 0);

    /// <summary>velocity.y의 값이 0보다 작거나 같은지 확인</summary>
    private bool IsLowVelocityY() => Player.playerRigidbody.velocity.y <= 0;

    /// <summary>현재 닿고 있는 땅이 있는지 확인</summary>
    private bool IsZeroGround() => (Player.playerCheckGround.GroundPlaneCount + Player.playerCheckGround.GroundPlatformCount) == 0;

    /// <summary>해당 SlotKey의 스킬이 사용 가능한지 확인</summary>
    private bool IsSkillCanUse(SkillSlotKey slotKey) => Player.DictSlotKeyToSkill[slotKey].CanUse;

    /// <summary>MoveDir.y의 값이 0보다 작은지 확인</summary>
    private bool IsDownMoveDirY() => Player.NetworkInput.MoveDir.y < 0;

    /// <summary>현재 대시 카운트가 0보다 큰지 확인</summary>
    private bool IsCanDashCount() => Player.PlayerData.PlayerAirData.CurDashCount > 0;

    /// <summary>현재 닿고 있는 GroundPlane이 0인지 확인</summary>
    private bool IsZeroGroundPlane() => Player.playerCheckGround.GroundPlaneCount == 0;


    /// <summary>
    /// Walk State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectWalkState()
    {
        if (!(IsZeroMoveDirX()) &&          //움직이는 입력값 X가 0이 아니라면
            IsCanJump() &&                  //점프가 가능한 상태라면
            !(IsTrigger()) &&               //IsTrigger가 비활성화 상태라면
            IsZeroVelocityY())              //velocitty Y의 힘이 0이라면
        {
            if (Player.Runner.IsServer)
                ChangeState(WalkState);         //WalkState로 전환
            return true;
        }
        return false;
    }


    /// <summary>
    /// Fall State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectFallState()
    {
        if (!(IsCanJump()))                 //점프가 가능하지 않다면
        {
            if (Player.Runner.IsServer)
                ChangeState(FallState);         //FallState로 전환
            return true;
        }
        return false;
    }


    public bool ConnectRigidbodyFallState()
    {
        if(IsLowVelocityY())                //velocityY값이 0보다 작거나 같다면
        {
            if (Player.Runner.IsServer)
                ChangeState(FallState);         //FallState로 전환
            return true;
        }
        return false;
    }


    /// <summary>
    /// Dash State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectDashState()
    {
        if (IsZeroMoveDir()) return false;                  //아무런 이동 키를 입력하고 있지 않다면 검사 취소

        if (Player.NetworkInput.IsSkillZ &&                        //해당 스킬 키를 입력하고 있다면
            IsSkillCanUse(SkillSlotKey.Z) &&                //해당 스킬이 사용가능하다면
            (!(IsDownMoveDirY()) || IsZeroGround()) &&      //공중에 뜬 상태거나 아래키를 누르고 있지 않다면
            IsCanDashCount())                               //대시 카운트가 존재한다면
        {
            if (Player.Runner.IsServer)
                ChangeState(PlayerSkillEnterStateDict[SkillSlotKey.Z]); //Z스킬 State로 전환
            return true;
        }
        return false;
    }


    /// <summary>
    /// Jump State에 진입 가능 여부를 확인하고 전환하는 메서드, DownJump도 확인함.
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectJumpState()
    {
        if(Player.NetworkInput.IsJump &&           //점프키를 입력했다면
            Player.PlayerData.PlayerAirData.CanJump())  //  남은 점프 횟수가 있다면
        {
            if (!(IsDownMoveDirY()))        //아래키를 입력하고 있지 않다면
            {
                if (Player.Runner.IsServer)
                    ChangeState(JumpState);     //JumpState로 전환
                return true;
            }
            
            if(IsZeroGroundPlane())         //닿고 있는 GroundPlane의 갯수가 0이라면
            {
                IsTriggerTrue();            //isTrigger을 true로 전환
                if (Player.Runner.IsServer)
                    ChangeState(FallState);     //FallState로 전환 
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Idle State에 진입 가능 여부를 확인하고 전환하는 메서드
    /// </summary>
    /// <returns>true면 Action 종료, false면 Action 계속</returns>
    public bool ConnectIdleState()
    {
        if (IsZeroMoveDirX() &&         //움직이는 입력값X가 0이라면
            IsCanJump() &&              //점프가 가능한 상태라면
            !(IsTrigger()) &&           //Trigger가 비활성화 상태라면
            IsZeroVelocityY())          //velocityY값이 0이라면
        {
            if (Player.Runner.IsServer)
                ChangeState(IdleState);     //IdleState로 전환
            return true;
        }
        return false;
    }

    public void CheckHoldSkillStop(IPlayerState state, System.Func<bool> isAction)
    {
        if (currentState == state)
        {
            if (!(isAction()) /*&& Player.SkillTrigger.HoldSkillCoroutine != null*/)//TODO: 홀드스킬 취소 나중에 추가 필요
            {
                if (Player.Runner.IsServer)
                    ChangeState(IdleState);
                return;
            }
        }
    }


    public int GetIntDictStateToInit(IPlayerState state)
    {
        foreach (var item in DictIntToState)
        {
            if (item.Value == state)
            {
                return item.Key;
            }
        }
        return -1;
    }

    private Coroutine downJumpCooldownCoroutine;

    public void StartDownJumpCooldown(float delay = 0.1f)
    {
        if (downJumpCooldownCoroutine != null)
            Player.StopCoroutine(downJumpCooldownCoroutine);

        downJumpCooldownCoroutine = Player.StartCoroutine(ResetDownJump(delay));
    }

    private IEnumerator ResetDownJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        DidDownJump = false;
    }
}
