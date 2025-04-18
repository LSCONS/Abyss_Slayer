using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RemoteZoneSkill", menuName = "SkillRefactory/Range/RemoteZoneSkill")]

public class RemoteZoneSkill : RangeAttackSkill
{

    [SerializeField] private float sizeX = 1;
    [SerializeField] private float sizeY = 1;
    [SerializeField] private float tickRate = 0.5f; // 몇 초 마다?
    [SerializeField] private float duration = 3f; // 얼마나 유지?
    [SerializeField] private LayerMask targetLayer; // 타겟 설정

    [Header("플레이어 기준 offset")]
    [SerializeField] private float spawnOffset = 5f; // 플레이어 기준 거리



    public override void UseSkill()
    {
        Vector2 dir = player.SpriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector3 spawnPos = player.transform.position + (Vector3)(dir * spawnOffset);


        // 풀에서 ZoneAOE 꺼내기
        var zone = PoolManager.Instance.Get<ZoneAOE>();
        zone.Init(spawnPos, sizeX, sizeY, tickRate, duration, Damage, targetLayer);
    }



}
