using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MageSkillD", menuName = "SkillRefactory/Range/ShockWave")]
public class MShockWaveSkill : RangeAttackSkill
{
    [SerializeField] private int shockCount = 6;
    [SerializeField] private float shockOffsetWidth = 1.0f;
    [SerializeField] private float shockOffsetTime = 0.5f;

    public override void UseSkill()
    {
        base.UseSkill();
        player.SkillTrigger.HoldSkillCoroutine = CoroutineManager.Instance.StartCoroutineEnter(ShockWave());
    }

    public IEnumerator ShockWave()
    {
        WaitForSeconds Wait = new WaitForSeconds(shockOffsetTime);

        Vector3 dir = new Vector3(PlayerFrontXNormalized() * 1.5f, 0.5f, 0);
        Vector3 startPos = player.transform.position + dir;

        for (int i = 0; i < shockCount; i++)
        {
            PoolManager.Instance.Get<MShockWave>().Init(startPos + (Vector3.right * shockOffsetWidth * (i + 1)), Damage);
        }

        yield return Wait;
    }
}
