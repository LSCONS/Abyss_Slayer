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

    [Header("AbnomalParameterName")]
    [SerializeField] private string abnomalParameterName    = "@Abnomal";
    [SerializeField] private string deadParameterName       = "Dead";


    public int GroundParameterHash      { get; private set; }
    public int IdleParameterHash        { get; private set; }
    public int WalkParameterHash        { get; private set; }

    public int AirParematerHash         { get; private set; }
    public int JumpParameterHash        { get; private set; }
    public int FallParameterHash        { get; private set; }

    public int SkillParameterHash       { get; private set; }
    public int X_SkillParameterHash     { get; private set; }
    public int Z_SkillParameterHash     { get; private set; }
    public int A_SkillParameterHash     { get; private set; }
    public int S_SkillParameterHash     { get; private set; }
    public int D_SkillParameterHash     { get; private set; }

    public int AbnomalParameterHash     { get; private set; }
    public int DeadParameterHash        { get; private set; }

    public int Z_SkillAnimationHash     { get; private set; }
    public int X_SkillAnimationHash     { get; private set; }
    public int A_SkillAnimationHash     { get; private set; }
    public int S_SkillAnimationHash     { get; private set; }
    public int D_SkillAnimationHash     { get; private set; }

    

    public void Init()
    {
        GroundParameterHash     = Animator.StringToHash(groundParameterName);
        IdleParameterHash       = Animator.StringToHash(idleParameterName);        
        WalkParameterHash       = Animator.StringToHash(walkParameterName);

        AirParematerHash        = Animator.StringToHash(airParematerName);
        JumpParameterHash       = Animator.StringToHash(jumpParameterName);       
        FallParameterHash       = Animator.StringToHash(fallParameterName);

        SkillParameterHash      = Animator.StringToHash(skillParameterName);
        X_SkillParameterHash    = Animator.StringToHash(X_SkillParameterName);
        Z_SkillParameterHash    = Animator.StringToHash(Z_SkillParameterName);     
        A_SkillParameterHash    = Animator.StringToHash(A_SkillParameterName);     
        S_SkillParameterHash    = Animator.StringToHash(S_SkillParameterName);     
        D_SkillParameterHash    = Animator.StringToHash(D_SkillParameterName);

        AbnomalParameterHash    = Animator.StringToHash(abnomalParameterName);
        DeadParameterHash       = Animator.StringToHash(deadParameterName);

        Z_SkillAnimationHash    = Animator.StringToHash(Z_SkillAnimationName);
        X_SkillAnimationHash    = Animator.StringToHash(X_SkillAnimationName);
        A_SkillAnimationHash    = Animator.StringToHash(A_SkillAnimationName);
        S_SkillAnimationHash    = Animator.StringToHash(S_SkillAnimationName);
        D_SkillAnimationHash    = Animator.StringToHash(D_SkillAnimationName);
    }                                               
}                                                   
