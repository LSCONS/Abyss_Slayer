using UnityEngine;

[CreateAssetMenu(fileName = "MageAttackSkill", menuName = "SkillRefactory/Range/MageAttack")]
public class MageAttackSkill : Skill
{
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public LayerMask TargetLayer { get; private set; } // 타겟 레이어

    public override void UseSkill()
    {
        base.UseSkill();
        Vector3 dirX = new Vector3(PlayerFrontXNomalized() * 1.5f, 0 ,0); // 플레이어 방향 계산

        Debug.DrawRay(PlayerPosition() + dirX, dirX * Range, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(PlayerPosition() + dirX, dirX, Range, TargetLayer);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            hit.collider.GetComponent<Boss>().Damage((int)Damage); // 데미지 전달
        }
    }
}
