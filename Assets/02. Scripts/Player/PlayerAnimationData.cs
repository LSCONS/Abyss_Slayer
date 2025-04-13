using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimationData", menuName = "Player/AnimationData")]
public class PlayerAnimationData : ScriptableObject
{
    [Header("GroundParameterName")]
    [SerializeField] private string groundParameterName     = "@Ground";
    [SerializeField] private string idleParameterName       = "Idle";
    [SerializeField] private string walkParameterName       = "Walk";

    [Header("AirParameterName")]
    [SerializeField] private string airParematerName        = "@Air";
    [SerializeField] private string jumpParameterName       = "Jump";
    [SerializeField] private string fallParameterName       = "Fall";

    [Header("SkillParameterName")]
    [SerializeField] private string skillParameterName      = "@Skill";
    [SerializeField] private string X_SkillParameterName    = "X_Skill";
    [SerializeField] private string Z_SkillParameterName    = "Z_Skill";
    [SerializeField] private string A_SkillParameterName    = "A_Skill";
    [SerializeField] private string S_SkillParameterName    = "S_Skill";
    [SerializeField] private string D_SkillParameterName    = "D_Skill";

    [Header("AnimationStateName")]
    [SerializeField] private string Z_SkillAnimationName    = "Player_SkillZ";
    [SerializeField] private string X_SkillAnimationName    = "Player_SkillX";
    [SerializeField] private string A_SkillAnimationName    = "Player_SkillA";
    [SerializeField] private string S_SkillAnimationName    = "Player_SkillS";
    [SerializeField] private string D_SkillAnimationName    = "Player_SkillD";

    public int groundParameterHash      { get; private set; }
    public int idleParameterHash        { get; private set; }
    public int walkParameterHash        { get; private set; }

    public int airParematerHash         { get; private set; }
    public int jumpParameterHash        { get; private set; }
    public int fallParameterHash        { get; private set; }

    public int skillParameterHash       { get; private set; }
    public int X_SkillParameterHash     { get; private set; }
    public int Z_SkillParameterHash     { get; private set; }
    public int A_SkillParameterHash     { get; private set; }
    public int S_SkillParameterHash     { get; private set; }
    public int D_SkillParameterHash     { get; private set; }

    public int Z_SkillAnimationHash     { get; private set; }
    public int X_SkillAnimationHash     { get; private set; }
    public int A_SkillAnimationHash     { get; private set; }
    public int S_SkillAnimationHash     { get; private set; }
    public int D_SkillAnimationHash     { get; private set; }

    public void Init()
    {
        groundParameterHash     = Animator.StringToHash(groundParameterName);
        idleParameterHash       = Animator.StringToHash(idleParameterName);        
        walkParameterHash       = Animator.StringToHash(walkParameterName);

        airParematerHash        = Animator.StringToHash(airParematerName);
        jumpParameterHash       = Animator.StringToHash(jumpParameterName);       
        fallParameterHash       = Animator.StringToHash(fallParameterName);

        skillParameterHash      = Animator.StringToHash(skillParameterName);
        X_SkillParameterHash    = Animator.StringToHash(X_SkillParameterName);
        Z_SkillParameterHash    = Animator.StringToHash(Z_SkillParameterName);     
        A_SkillParameterHash    = Animator.StringToHash(A_SkillParameterName);     
        S_SkillParameterHash    = Animator.StringToHash(S_SkillParameterName);     
        D_SkillParameterHash    = Animator.StringToHash(D_SkillParameterName);

        Z_SkillAnimationHash    = Animator.StringToHash(Z_SkillAnimationName);
        X_SkillAnimationHash    = Animator.StringToHash(X_SkillAnimationName);
        A_SkillAnimationHash    = Animator.StringToHash(A_SkillAnimationName);
        S_SkillAnimationHash    = Animator.StringToHash(S_SkillAnimationName);
        D_SkillAnimationHash    = Animator.StringToHash(D_SkillAnimationName);
    }                                               
}                                                   
