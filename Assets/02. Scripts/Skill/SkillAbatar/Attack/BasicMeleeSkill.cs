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
        base.UseSkill();
        // 콜라이더 초기화
        if(meleeDamageCheck == null)
            meleeDamageCheck = player.PlayerMeleeCollider.GetComponent<MeleeDamageCheck>();

        MeleeDamageCheckData data = new MeleeDamageCheckData(player.PlayerRef, (int)slotKey, ColliderSize, ColliderOffset, TargetMask.value, 0, Damage, ColliderDuration, 100, (int)EHitEffectType.Normal, (int)EType.None);

        meleeDamageCheck.Rpc_BasicInit(data);
    }
}
