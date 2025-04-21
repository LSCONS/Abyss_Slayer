using UnityEngine;

[CreateAssetMenu(fileName = "MageAttackSkill", menuName = "SkillRefactory/Range/MageAttack")]
public class MageAttackSkill : RangeAttackSkill
{
    public override void UseSkill()
    {
        base.UseSkill();
        Vector3 dirX = new Vector3(PlayerFrontXNomalized() * 1.5f, 0 ,0); // 플레이어 방향 계산

        Debug.DrawRay(PlayerPosition() + dirX, dirX * Range, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(PlayerPosition() + dirX, dirX, Range, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            hit.collider.GetComponent<Boss>().Damage((int)Damage); // 데미지 전달
        }
    }
}
