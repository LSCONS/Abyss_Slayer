public enum SkillCategory
{
    BasicAtk,
    Heal,
    Damage, 
    Buff, 
    Debuff
}

public enum SkillSlotKey
{
    X,
    A,
    S,
    D,
    Z
}

public enum DamageType
{
    Physical,
    Magical,
    Hybrid
}

public enum EvasionType
{
    Dash,
    Teleport
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
