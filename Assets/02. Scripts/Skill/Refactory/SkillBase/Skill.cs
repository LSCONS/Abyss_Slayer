using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Skill : ScriptableObject
{
    [HideInInspector] public Player player;
    [field: SerializeField] public string SkillName { get; private set; } = "스킬 이름";
    [field: SerializeField] public string SkillDesription { get; private set; } = "스킬 설명";
    [field: SerializeField] public Sprite SkillIcon {get; private set;}

    [field: SerializeField] public bool CanUse { get; set; } = true;            //스킬 사용 가능 여부
    [field: SerializeField] public bool CanMove { get; private set; } = true;   //스킬 사용 중 움직임 가능 여부

    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; private set; }    //최대 쿨타임
        = new ReactiveProperty<float>(10f);
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; private set; }    //현재 쿨타임
        = new ReactiveProperty<float>(0f);
    [field: SerializeField] public ApplyState ApplyState { get; set; }      //연결해서 작동시킬 State 설정
    
    public void Init(Player player)
    {
        this.player = player;
    }


    public virtual void UseSkill()
    {

    }

    public float PlayerFrontXNomalized()
    {
        float x = player.SpriteRenderer.flipX ? -1f : 1f;
        return x;
    }

    public Vector3 PlayerPosition()
    {
        Vector3 playerPosition = player.transform.position;
        return playerPosition;
    }
}
