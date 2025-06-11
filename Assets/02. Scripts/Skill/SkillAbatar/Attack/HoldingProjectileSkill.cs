using System.Collections;
using UnityEngine;

/// <summary>
/// 스킬 키를 꾹 누르면 탄환을 계속 발사하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewHoldingProjectileSkill", menuName = "Skill/ProjectileAttack/Holding")]
public class HoldingProjectileSkill : ProjectileAttackSkill
{
    [field: Header("탄환 개수")]
    [field: SerializeField] public int ProjectileCount { get; private set; } = 50;
    [field: Header("발사 지연 시간")]
    [field: SerializeField] public float ShotDelay { get; private set; } = 0.1f;

    private Vector3 distanceY = new Vector3(0, 0.25f, 0);

    public override void UseSkill()
    {
        base.UseSkill();
        player.StartHoldSkillCoroutine(FireArrows(), null);
    }

    // 화살 발사 코루틴
    public IEnumerator FireArrows()
    {
        // GC 최적화를 위한 WaitForSeconds 캐싱
        WaitForSeconds wait = new WaitForSeconds(ShotDelay);

        for (int i = 0; i < ProjectileCount; i++)
        {
            //소리 시작
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip);

            // 플레이어가 바라보는 방향 계산
            Vector2 dir = new Vector2(PlayerFrontXNormalized(), 0);

            // 일정 범위 내에서 화살 랜덤 생성
            float randomYSpawn = Random.Range(-0.3f, 0.3f);

            // 화살 생성 위치 설정
            Vector3 spawnPos = player.transform.position + (Vector3)(dir * 1.5f) + new Vector3(0, randomYSpawn, 0);

            // 버프 상태일 경우 추가 화살 생성
            if (player.DictBuffTypeToBuffSkill.ContainsKey(EBuffType.RogueDoubleShot) && player.DictBuffTypeToBuffSkill[EBuffType.RogueDoubleShot].IsApply)
            {
                float x = Random.Range(-1f, 1f);
                Vector3 distanceX = new Vector3(x, 0, 0);
                PoolManager.Instance.Get<RogueProjectile>().Rpc_Init(player.PlayerRef, spawnPos + distanceY + distanceX, dir, Range, Speed, Damage);

                x = Random.Range(-1f, 1f);
                distanceX = new Vector3(x, 0, 0);
                PoolManager.Instance.Get<RogueProjectile>().Rpc_Init(player.PlayerRef, spawnPos - distanceY + distanceX, dir, Range, Speed, Damage * DamageMultiple);
            }
            else
            {
                PoolManager.Instance.Get<RogueProjectile>().Rpc_Init(player.PlayerRef, spawnPos, dir, Range, Speed, Damage);
            }

            // 다음 화살 발사까지 대기
            yield return wait;
        }
        player.StopHoldSkillActionCoroutine();
    }
}
