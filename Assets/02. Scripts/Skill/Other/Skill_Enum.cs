public enum SkillCategory
{
    None,
    BasicAtk,
    Heal,
    Buff, 
    Debuff,
    Dash,
    Attack,
    Hold,
    Charge,
    DashAttack,
}

public enum SkillSlotKey
{
    X,
    A,
    S,
    D,
    Z,

    Move,
    Jump
}

public enum DamageType
{
    None,
    Physical,
    Magical,
    Hybrid
}

[System.Flags]
public enum ApplyState
{
    None = 0,
    IdleState = 1 << 0,      //가만히 있는 상태
    WalkState = 1 << 1,      //움직이고 있는 상태
    FallState = 1 << 2,      //공중에서 떨어지고 있는 상태
    DashState = 1 << 3,      //회피하고 있는 상태
    JumpState = 1 << 4,      //점프하고 있는 상태
}
