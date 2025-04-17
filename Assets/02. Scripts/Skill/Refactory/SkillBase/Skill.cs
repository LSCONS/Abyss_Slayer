using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Skill : ScriptableObject
{
    [HideInInspector]
    public Player player;
    public string skillName = "스킬 이름";
    public string info = "스킬 정보";
    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; set; } = new ReactiveProperty<float>(10f);
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; set; } = new ReactiveProperty<float>(0f);
    public bool canUse = true;
    public bool canMove = true;
    public Sprite icon;
    [field: SerializeField]public ApplyState ApplyState { get; set; }
    
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
