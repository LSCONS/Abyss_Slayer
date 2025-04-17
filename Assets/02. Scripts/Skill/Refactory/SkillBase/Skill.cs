using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Skill : ScriptableObject
{
    [HideInInspector] public Player player;
    [field: SerializeField] public string skillName {get; private set;} 
    [field: SerializeField] public string info {get; private set;}
    [field: SerializeField] public Sprite icon {get; private set;}

    public bool canUse = true;
    public bool canMove = true;
    
    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; private set; } = new ReactiveProperty<float>(10f);
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; private set; } = new ReactiveProperty<float>(0f);
    [field: SerializeField] public ApplyState ApplyState { get; set; }
    
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
