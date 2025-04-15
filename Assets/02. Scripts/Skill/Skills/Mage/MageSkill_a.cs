using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skill/Mage/Mage_a")]
public class MageSkill_a : SkillExecuter
{
    public int damage;
    [SerializeField] private GameObject target;

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

    private Transform TargetTransform => target?.transform;

    public override void Execute(Player user, Player target, SkillData skillData)
    {
        for(int i = 0; i < count; i++)
        {
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (count - 1)));
            degree = user.SpriteRenderer.flipX ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree); //오일러 각도로 각도계산

            degree *= Mathf.Deg2Rad; //오일러 각도를 라디우스로 변환
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR; //라디우스로 위치계산
            position = user.transform.TransformPoint(position);

            Transform targetTransform = TargetTransform ?? user.transform;
            PoolManager.Instance.Get<MageProjectile>().Init(damage, position, rotate, targetTransform, speed, homingPower, homingTime, homingCurve);
        }
    }
}
