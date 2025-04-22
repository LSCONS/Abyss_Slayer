using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 MoveDir;
    public bool IsJump;
    public bool IsSkillZ;
    public bool IsSkillX;
    public bool IsSkillA;
    public bool IsSkillS;
    public bool IsSkillD;
}
