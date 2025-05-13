using UnityEngine;

/// <summary>
/// Raycast를 이용해 직선에 닿는 첫 타겟에게 데미지를 주는 기능
/// </summary>
[CreateAssetMenu(fileName = "NewRaycastRangeSkill", menuName = "Skill/RangeAttack/RayCast")]
public class RaycastRangeSkill : RangeAttackSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Vector3 dir = new Vector3(PlayerFrontXNormalized() * 1.5f, 0 ,0); // 플레이어 방향 계산

        Debug.DrawRay(PlayerPosition() + dir, dir * 8, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(PlayerPosition() + dir, dir, 8, TargetLayer);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            hit.collider.GetComponent<IHasHealth>().Damage((int)Damage,hit.point.x); // 데미지 전달
        }
    }
}
