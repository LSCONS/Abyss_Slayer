using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BasicMeleeAttackSkill", menuName = "SkillRefactory/Melee/BasicMeleeAttackSkill")]
public class BasicMeleeAttackSkill : MeleeAttackSkill
{
    [Header("콜라이더 크기")]
    [SerializeField] private float sizeX;
    [SerializeField] private float sizeY;
    [SerializeField] private Vector2 offset;

    [Header("일반공격 쿨다운")]
    public float cooldownBonus;

    public override void UseSkill()
    {
        Debug.Log("때리기");
        player.PlayerMeleeCollider.enabled = true;

        // 콜라이더 초기화
        MeleeDamageCheck damageCheck = player.PlayerMeleeCollider.GetComponent<MeleeDamageCheck>();
        damageCheck.Init(player, sizeX, sizeY, offset, Damage, typeof(BossHitEffect), ColliderDuration);

        // 콜라이더 온오프
        player.StartCoroutine(player.EnableMeleeCollider(player.PlayerMeleeCollider, ColliderDuration));


    }
}
