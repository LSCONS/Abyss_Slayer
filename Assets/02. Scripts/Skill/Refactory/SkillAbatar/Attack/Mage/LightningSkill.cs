using System.Collections;
using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "MageSkillS", menuName = "SkillRefactory/Range/Lightning")]
public class LightningSkill : RangeAttackSkill
{
    public float DamageDelay { get; private set; } = 0.05f;
    public int DamageCount { get; private set; } = 100;
    public override void UseSkill()
    {
        base.UseSkill();
        player.SkillTrigger.HoldSkillCoroutine = CoroutineManager.Instance.StartCoroutineEnter(Lightning());
    }

    public IEnumerator Lightning()
    {
        Vector3 dir = new Vector3(PlayerFrontXNormalized() * 1.5f, 0, 0);

        Instantiate(SkillEffect, player.transform.position + (Vector3)(dir * 4f), Quaternion.identity);

        WaitForSeconds Wait = new WaitForSeconds(DamageDelay);

        for (int i = 0; i < DamageCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position + dir, dir, Range, TargetLayer);
            if(hit.collider != null)
            {
                if(hit.collider.TryGetComponent<Boss>(out Boss boss))
                {
                    boss.Damage(Damage);
                }
            }
            yield return Wait;
        }

        Destroy(SkillEffect.gameObject);
        player.SkillTrigger.HoldSkillCoroutine = null;
    }
}
