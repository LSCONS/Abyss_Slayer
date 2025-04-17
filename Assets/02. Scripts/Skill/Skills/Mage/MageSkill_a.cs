using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skill/Mage/Mage_a")]
public class MageSkill_a : SkillExecuter
{
    public float damage;

    [Header("발사정보")]
    public int count;                       // 투사체 개수
    public float startCircleR;              // 투사체 시작 위치 반지름
    public float fireDegree;                // 중심 발사 각도
    public float spreadDegree;              // 투사체 퍼짐 각도

    [Header("투사체 정보")]
    public float homingPower;               // 유도 강도
    public float speed;                     // 투사체 속도
    public float homingTime;                // 유도 지속 시간
    public AnimationCurve homingCurve;      // 유도 세기 커브

    private MageSkillRangeVisualizer rangeVisualizer; // 범위 시각화 컴포넌트(디버그용)

    /// <summary>
    /// 스킬 실행
    /// </summary>
    /// <param name="user">플레이어</param>
    /// <param name="target">타겟</param>
    /// <param name="skillData">스킬 데이터</param>
    public override void Execute(Player user, Boss target, SkillData skillData)
    {
        // 범위 시각화 컴포넌트를 플레이어에 추가하고 설정(디버그용)
        if (rangeVisualizer == null)
        {
            rangeVisualizer = user.gameObject.AddComponent<MageSkillRangeVisualizer>();
        }
        rangeVisualizer.SetRange(skillData.targetingData.range);

        // 타겟이 없을 경우 범위 내에서 탐색
        if (target == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(user.transform.position, skillData.targetingData.range, LayerMask.GetMask("Enemy"));
            
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<Boss>(out Boss boss))
                {
                    target = boss;
                    break;
                }
            }
        }

        // 탐색 후에도 타겟이 없다면 스킬 발동 취소
        if (target == null)
        {
            Debug.LogAssertion("타겟이 없습니다.");
            return;
        }

        // 지정된 개수만큼 투사체 생성
        for (int i = 0; i < count; i++)
        {
            // 각 투사체의 발사 각도를 계산
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (count - 1)));
            degree = user.SpriteRenderer.flipX ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree);

            // 투사체 생성 위치 계산 (원을 기준으로 배치)
            degree *= Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;
            position = user.transform.TransformPoint(position);

            // 유도탄 초기화 데이터 투사체에 전달
            PoolManager.Instance.Get<MageProjectile>().Init(damage, position, rotate, target.transform, speed, homingPower, homingTime, homingCurve);
        }
    }
}
