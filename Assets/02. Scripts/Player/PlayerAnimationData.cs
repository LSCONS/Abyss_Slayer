using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnimationData
{
    [SerializeField] private string groundParameterName         = "@Ground";
    [SerializeField] private string idleParameterName           = "Idle";
    [SerializeField] private string walkParameterName           = "Walk";

    [SerializeField] private string airParematerName            = "@Air";
    [SerializeField] private string jumpParameterName           = "Jump";
    [SerializeField] private string fallParameterName           = "Fall";

    [SerializeField] private string attackParameterName         = "@Skill";
    [SerializeField] private string commonAttackParameterName   = "CommonAttack";
    [SerializeField] private string Z_SkillParameterName        = "Z_Skill";
    [SerializeField] private string A_SkillParameterName        = "A_Skill";
    [SerializeField] private string S_SkillParameterName        = "S_Skill";
    [SerializeField] private string D_SkillParameterName        = "D_Skill";

    public int groundParameterHash          { get; private set; }
    public int idleParameterHash            { get; private set; }
    public int walkParameterHash            { get; private set; }
    public int airParematerHash             { get; private set; }
    public int jumpParameterHash            { get; private set; }
    public int fallParameterHash            { get; private set; }
    public int attackParameterHash          { get; private set; }
    public int commonAttackParameterHash    { get; private set; }
    public int Z_SkillParameterHash         { get; private set; }
    public int A_SkillParameterHash         { get; private set; }
    public int S_SkillParameterHash         { get; private set; }
    public int D_SkillParameterHash         { get; private set; }


    public PlayerAnimationData()
    {
        Init();
    }

    private void Init()
    {
        groundParameterHash =Animator.StringToHash( groundParameterName);
        idleParameterHash = Animator.StringToHash(idleParameterName);        
        walkParameterHash = Animator.StringToHash(walkParameterName);
        airParematerHash = Animator.StringToHash(airParematerName);
        jumpParameterHash = Animator.StringToHash(jumpParameterName);       
        fallParameterHash = Animator.StringToHash(fallParameterName);
        attackParameterHash = Animator.StringToHash(attackParameterName);
        commonAttackParameterHash = Animator.StringToHash(commonAttackParameterName);
        Z_SkillParameterHash = Animator.StringToHash(Z_SkillParameterName);     
        A_SkillParameterHash = Animator.StringToHash(A_SkillParameterName);     
        S_SkillParameterHash = Animator.StringToHash(S_SkillParameterName);     
        D_SkillParameterHash = Animator.StringToHash(D_SkillParameterName);     
    }                                               
}                                                   
