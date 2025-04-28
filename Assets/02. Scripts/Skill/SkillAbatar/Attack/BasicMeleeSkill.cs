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
        Debug.Log("때리기");
        player.PlayerMeleeCollider.enabled = true;

        // 콜라이더 초기화
        if(meleeDamageCheck == null)
            meleeDamageCheck = player.PlayerMeleeCollider.GetComponent<MeleeDamageCheck>();

        MeleeDamageCheckData data = new MeleeDamageCheckData(player, this, ColliderSize, ColliderOffset, TargetMask, Damage, ColliderDuration, 0, false, typeof(BossHitEffect));


        meleeDamageCheck.Init(data);

        // 콜라이더 온오프
        player.StartCoroutine(player.EnableMeleeCollider(player.PlayerMeleeCollider, ColliderDuration));
    }
}
