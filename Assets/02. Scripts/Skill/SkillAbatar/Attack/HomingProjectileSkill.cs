using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

/// <summary>
/// 특정 타겟을 향해 유도 탄환을 발사하는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewHomingBombProjectileSkill", menuName = "Skill/ProjectileAttack/HomingBomb")]
public class HomingProjectileSkill : ProjectileAttackSkill
{
    [field: Header("투사체 개수")]
    [field: SerializeField] public int ProjectileCount { get; private set; } = 1;       // 투사체 개수
    [field: Header("투사체 시작 위치 반지름")]
    [field: SerializeField] public float StartCircleR { get; private set; } = 1f;       // 투사체 시작 위치 반지름
    [field: Header("중심 발사 각도")]
    [field: SerializeField] public float FireDegree { get; private set; } = 1f;         // 중심 발사 각도
    [field: Header("투사체 퍼짐 각도")]
    [field: SerializeField] public float SpreadDegree { get; private set; } = 1f;       // 투사체 퍼짐 각도
    [field: Header("유도 강도")]
    [field: SerializeField] public float HomingPower { get; private set; } = 1f;        // 유도 강도
    [field: Header("유도 지속 시간")]
    [field: SerializeField] public float HomingTime { get; private set; } = 10f;        // 유도 지속 시간
    [field: Header("유도 세기 커브")]
    [field: SerializeField] public EAniamtionCurve EHomingCurve { get; private set; }     // 유도 세기 커브
    [field: Header("타겟 레이어")]
    [field: SerializeField] public LayerMask TargetLayer { get; private set; }          // 타겟 레이어

    private MageSkillRangeVisualizer rangeVisualizer; // 범위 시각화
    private Boss target;
    

    public override void UseSkill()
    {
        base.UseSkill();
        
        // 범위 시각화 컴포넌트를 플레이어에 추가하고 설정(디버그용)
        if (rangeVisualizer == null)
        {
            rangeVisualizer = player.gameObject.AddComponent<MageSkillRangeVisualizer>();
        }
        rangeVisualizer.SetRange(Range);

        //범위 내에서 타겟 탐색
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        Collider2D collider = scene2D.OverlapCircle(player.transform.position, Range, TargetLayer);

        if (collider != null && collider.TryGetComponent<Boss>(out Boss boss))
        {
            target = boss;
        }

        // 탐색 후에도 타겟이 없다면 스킬 발동 취소
        if (target == null)
        {
            CurCoolTime.Value = 0.5f;
            CanUse = false;
            return;
        }


        // 지정된 개수만큼 투사체 생성
        for (int i = 0; i < ProjectileCount; i++)
        {
            // 각 투사체의 발사 각도를 계산
            float degree = FireDegree - ((SpreadDegree / 2) - (i * SpreadDegree / (ProjectileCount)));
            degree = player.IsFlipX ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree);

            // 투사체 생성 위치 계산 (원을 기준으로 배치)
            degree *= Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * StartCircleR;
            position = player.transform.TransformPoint(position);

            // 유도탄 초기화 데이터 투사체에 전달
            PoolManager.Instance.Get<MageProjectile>().Rpc_Init(player.PlayerRef, Damage, position, rotate, Speed, HomingPower, HomingTime, (int)EHomingCurve);
        }
        target = null;
    }
}
