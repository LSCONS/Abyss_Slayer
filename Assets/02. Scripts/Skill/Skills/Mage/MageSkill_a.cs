using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skill/Mage/Mage_a")]
public class MageSkill_a : SkillExecuter
{
    public float damage;

    [Header("발사정보")]
    public int count;
    public float startCircleR;
    public float fireDegree;
    public float spreadDegree;

    [Header("투사체 정보")]
    public float homingPower;
    public float speed;
    public float homingTime;
    public AnimationCurve homingCurve;

    private MageSkillRangeVisualizer rangeVisualizer;

    public override void Execute(Player user, Boss target, SkillData skillData)
    {
        // 범위 시각화 컴포넌트 설정
        if (rangeVisualizer == null)
        {
            rangeVisualizer = user.gameObject.AddComponent<MageSkillRangeVisualizer>();
        }
        rangeVisualizer.SetRange(skillData.targetingData.range);

        // 타겟이 지정되지 않은 경우, 범위 내에서 보스를 찾음
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

        // 타겟이 여전히 null이면 스킬을 발동하지 않음
        if (target == null)
        {
            Debug.LogAssertion("타겟이 없습니다.");
            return;
        }

        for(int i = 0; i < count; i++)
        {
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (count - 1)));
            degree = user.SpriteRenderer.flipX ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree);

            degree *= Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;
            position = user.transform.TransformPoint(position);

            PoolManager.Instance.Get<MageProjectile>().Init(damage, position, rotate, target.transform, speed, homingPower, homingTime, homingCurve);
        }
    }
}
