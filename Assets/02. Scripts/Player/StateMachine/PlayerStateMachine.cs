using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerDeadState DieState { get; private set; }

    public PlayerSkillZState SkillZState { get; private set; }
    public PlayerSkillXState SkillXState { get; private set; }
    public PlayerSkillAState SkillAState { get; private set; }
    public PlayerSkillSState SkillSState { get; private set; }
    public PlayerSkillDState SkillDState { get; private set; }

    public StoppableAction SkipAttackAction = new();
    public StoppableAction EndAttackAction = new();

    public bool IsDash { get; set; } = false;       //플레이어가 현재 대시 상태인지 출력하는 변수
    public bool CanMove { get; set; } = true;
    public AnimatorStateInfo AnimatorInfo { get; set; }
    public bool IsCompareState { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;

    public PlayerStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);
        DieState = new PlayerDeadState(this);

        SkillZState = new PlayerSkillZState(this);
        SkillAState = new PlayerSkillAState(this);
        SkillSState = new PlayerSkillSState(this);
        SkillDState = new PlayerSkillDState(this);
        SkillXState = new PlayerSkillXState(this);

        IdleState.Init();
        WalkState.Init();
        JumpState.Init(); 
        FallState.Init(); 
        DieState.Init(); 
        SkillZState.Init();
        SkillAState.Init();
        SkillSState.Init();
        SkillDState.Init();
        SkillXState.Init();

        SkipAttackAction.AddListener(ConnectJumpState);
        SkipAttackAction.AddListener(ConnectDashState);

        EndAttackAction.AddListener(ConnectFallState);
        EndAttackAction.AddListener(ConnectIdleState);
        EndAttackAction.AddListener(ConnectWalkState);

        MovementSpeed = Player.playerData.PlayerGroundData.BaseSpeed;
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

        if ((ApplyState.DashState | applyState) == applyState)
            SkillZState.MoveAction.AddListener(() => ConnectAttackAction(isAction(), state, skillData));

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
    private bool IsZeroMoveDirX() => Player.input.MoveDir.x == 0;

    /// <summary>MoveDir의 X값이 0인지 확인</summary>
    private bool IsZeroMoveDir() => Player.input.MoveDir == Vector2.zero;

    /// <summary>점프가 가능한 상태인지 확인</summary>
    private bool IsCanJump() => Player.playerCheckGround.CanJump;

    /// <summary>isTrigger가 어떤 상태인지 확인</summary>
    private bool IsTrigger() => Player.playerGroundCollider.isTrigger;

    /// <summary>isTrigger을 true로 변경</summary>
    private void IsTriggerTrue() => Player.playerGroundCollider.isTrigger = true;

    /// <summary>velocity.y의 값이 0에 가까운지 확인</summary>
    private bool IsZeroVelocityY() => Mathf.Approximately(Player.playerRigidbody.velocity.y, 0);

    /// <summary>현재 닿고 있는 땅이 있는지 확인</summary>
    private bool IsZeroGround() => (Player.playerCheckGround.GroundPlaneCount + Player.playerCheckGround.GroundPlatformCount) == 0;

    /// <summary>해당 SlotKey의 스킬이 사용 가능한지 확인</summary>
    private bool IsSkillCanUse(SkillSlotKey slotKey) => Player.equippedSkills[slotKey].CanUse;

    /// <summary>MoveDir.y의 값이 0보다 작은지 확인</summary>
    private bool IsDownMoveDirY() => Player.input.MoveDir.y < 0;

    /// <summary>현재 대시 카운트가 0보다 큰지 확인</summary>
    private bool IsCanDashCount() => Player.playerData.PlayerAirData.CurDashCount > 0;

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

        if (Player.input.IsSkillZ &&                        //해당 스킬 키를 입력하고 있다면
            IsSkillCanUse(SkillSlotKey.Z) &&                //해당 스킬이 사용가능하다면
            (!(IsDownMoveDirY()) || IsZeroGround()) &&      //공중에 뜬 상태거나 아래키를 누르고 있지 않다면
            IsCanDashCount())                               //대시 카운트가 존재한다면
        {
            ChangeState(SkillZState);                       //Z스킬 State로 전환
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
        if(Player.input.IsJump &&           //점프키를 입력했다면
            IsCanJump() &&                  //점프가 가능한 상태라면
            IsZeroVelocityY())              //Y축으로 이동하는 힘이 0이라면
        {
            if (!(IsDownMoveDirY()))        //아래키를 입력하고 있지 않다면
            {
                ChangeState(JumpState);     //JumpState로 전환
                return true;
            }
            
            if(IsZeroGroundPlane())         //닿고 있는 GroundPlane의 갯수가 0이라면
            {
                IsTriggerTrue();            //isTrigger을 true로 전환
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
            ChangeState(IdleState);     //IdleState로 전환
            return true;
        }
        return false;
    }
}
