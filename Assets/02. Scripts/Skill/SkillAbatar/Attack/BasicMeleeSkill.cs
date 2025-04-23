using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBasicMeleeSkill", menuName = "Skill/MeleeAttack/Basic")]
public class BasicMeleeSkill : MeleeAttackSkill
{
    [field: Header("콜라이더 크기")]
    [field: SerializeField] public Vector2 ColliderSize { get; private set; }
    [field: Header("콜라이더 위치")]
    [field: SerializeField] public Vector2 ColliderOffset {  get; private set; }

    public override void UseSkill()
    {
        Debug.Log("때리기");
        player.playerMeleeCollider.enabled = true;

        // 콜라이더 초기화
        MeleeDamageCheck damageCheck = player.playerMeleeCollider.GetComponent<MeleeDamageCheck>();
        damageCheck.Init(player, this, ColliderSize, ColliderOffset, Damage, typeof(BossHitEffect), ColliderDuration);

        // 콜라이더 온오프
        player.StartCoroutine(player.EnableMeleeCollider(player.playerMeleeCollider, ColliderDuration));
    }
}
