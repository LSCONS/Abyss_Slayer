using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationData : MonoBehaviour
{
    [SerializeField] private string groundParameterName = "@Ground";
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string walkParameterName = "Walk";

    [SerializeField] private string airParematerName = "@Air";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string fallParameterName = "Fall";

    [SerializeField] private string attackParameterName = "@Attack";
    [SerializeField] private string comboAttackParameterName = "ComboAttack";

    [SerializeField] private string skillParameterName = "@Skill";
    [SerializeField] private string skill1ParameterName = "Skill1";
    [SerializeField] private string skill2ParameterName = "Skill2";
    [SerializeField] private string skill3ParameterName = "Skill3";
}
