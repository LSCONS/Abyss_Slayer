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
    private MeleeDamageCheck meleeDamageCheck = null;

    public override void UseSkill()
    {
        // 콜라이더 초기화
        if(meleeDamageCheck == null)
            meleeDamageCheck = player.PlayerMeleeCollider.GetComponent<MeleeDamageCheck>();

        MeleeDamageCheckData data = new MeleeDamageCheckData(player, this, ColliderSize, ColliderOffset, TargetMask,0, Damage, ColliderDuration, 0, false, typeof(BossHitEffect));

        meleeDamageCheck.BasicInit(data);
    }
}
