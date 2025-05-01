using UnityEngine;

/// <summary>
/// 탄환을 하나 발사하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewOnceProjectileSkill", menuName = "Skill/ProjectileAttack/Once")]
public class OnceProjectileSkill : ProjectileAttackSkill
{
    private Vector3 distanceY = new Vector3(0, 0.25f, 0); // 화살 y축 위치
    
    public override void UseSkill()
    {
        base.UseSkill(); 
        Vector3 dirX = new Vector3(PlayerFrontXNormalized() * 1.5f, 0 ,0); // 플레이어 방향 계산
        Vector3 spawnPos = PlayerPosition() + dirX; // 화살 생성 위치

        // 버프 상태일 경우 추가 화살 생성
        if (player.BuffDuration.ContainsKey(BuffType.RogueDoubleShot) && player.BuffDuration[BuffType.RogueDoubleShot].IsApply)
        {
            ThrowProjectile(spawnPos + distanceY, dirX, 0.85f); 
            ThrowProjectile(spawnPos - distanceY, dirX, 0.85f);
        }
        else
        {
            ThrowProjectile(spawnPos, dirX);
        }
    }
}
