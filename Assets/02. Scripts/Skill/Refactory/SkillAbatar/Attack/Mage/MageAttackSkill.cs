using UnityEngine;

[CreateAssetMenu(fileName = "MageSkillX", menuName = "SkillRefactory/Range/MageAttack")]
public class MageAttackSkill : RangeAttackSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Vector3 dir = new Vector3(PlayerFrontXNormalized() * 1.5f, 0 ,0); // 플레이어 방향 계산

        Debug.DrawRay(PlayerPosition() + dir, dir * Range, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(PlayerPosition() + dir, dir, Range, TargetLayer);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            hit.collider.GetComponent<Boss>().Damage((int)Damage); // 데미지 전달
        }
    }
}
