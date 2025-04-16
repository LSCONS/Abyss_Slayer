using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Skill : ScriptableObject
{
    [HideInInspector]
    public Player player;
    public string Name = "스킬 이름";
    public string Info = "스킬 정보";
    [field: SerializeField] public ReactiveProperty<float> MaxCoolTime { get; set; } = new ReactiveProperty<float>(10f);
    [field: SerializeField] public ReactiveProperty<float> CurCoolTime { get; set; } = new ReactiveProperty<float>(0f);
    public bool CanUse = true;
    public bool CanMove = true;
    
    public void Init(Player _player)
    {
        player = _player;
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
