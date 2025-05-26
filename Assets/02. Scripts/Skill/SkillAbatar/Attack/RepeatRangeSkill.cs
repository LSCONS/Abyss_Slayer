using Fusion;
using System.Collections;
using UnityEngine;

/// <summary>
/// RemoteZone 스킬을 코루틴을 이용해 연속적으로 사용하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewRepeatRangeSKill", menuName = "Skill/RangeAttack/RepeatRange")]
public class RepeatRangeSkill : RemoteZoneRangeSkill
{
    [field: Header("스킬을 반복할 횟수")]
    [field: SerializeField] public int SkillRepeatCount { get; private set; } = 5;
    [field: Header("스킬을 사용할 때마다 움직일 오브젝트 위치 값")]
    [field: SerializeField] public Vector2 MovePosition { get; private set; } = Vector2.zero;
    [field: Header("스킬을 반복할 딜레이 시간")]
    [field: SerializeField] public float SkillRepeatDelayTime { get; private set; } = 0.5f;

    public override void UseSkill()
    {

        Vector2 temp = MovePosition;
        if (SkillCategory == SkillCategory.Hold)
        {
            player.StartHoldSkillCoroutine(Repeat(), null);
        }
        else 
        {
            player.StartCoroutine(Repeat());
        }
    }

    public IEnumerator Repeat()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        PhysicsScene2D runner2D = runner.GetPhysicsScene2D();
        WaitForSeconds wait = new WaitForSeconds(SkillRepeatDelayTime);
        Vector2 copyMovePosition = MovePosition;
        Vector2 resultMovePosition = Vector2.zero;
        Vector3 playerPosition = PlayerPosition();
        float flipX = PlayerFrontXNormalized();

        // 풀에서 ZoneAOE 꺼내기
        for (int i = 0; i < 5; i++)
        {
            MovePosition = resultMovePosition;
            SoundManager.Instance.PlaySFX(EAudioClip);
            MeleeDamageCheckData data = new MeleeDamageCheckData
            (
                player.PlayerRef,
                (int)slotKey,
                ColliderSize,
                ColliderOffset,
                TargetLayer,
                0,
                Damage,
                ColliderDuration,
                TickRate, 
                (int)EEffectAnimatorController
            );
            PoolManager.Instance.Get<ZoneAOE>().RepeatInit(data, SpawnSize, SpawnOffset, MovePosition, flipX, playerPosition);
            Vector2 spawnPosition = (Vector2)playerPosition + new Vector2(SpawnOffset.x * flipX, SpawnOffset.y);
            Vector2 startPosition = spawnPosition + resultMovePosition * flipX;
            Vector2 colliderTotalSize = new Vector2(ColliderSize.x * SpawnSize.x, ColliderSize.y * SpawnSize.y);
            Vector2 pointXY = new Vector2(startPosition.x, startPosition.y + colliderTotalSize.y/2);
            if(!(runner2D.OverlapBox(pointXY, colliderTotalSize, 0, LayerData.EnemyLayerMask)))
            {
                resultMovePosition += copyMovePosition;
            }
            else
            {
                resultMovePosition += copyMovePosition / 4;
            }
                yield return wait;
        }
        if(SkillCategory == SkillCategory.Hold)
        {
            player.StopHoldSkillActionCoroutine();
        }
        else
        {
            MovePosition = copyMovePosition;
        }
    }
}
