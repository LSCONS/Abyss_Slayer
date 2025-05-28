using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxSphere2")]
public class FoxSphere2Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] int projectileCountPerAttack;
    [SerializeField] int AttackCount;
    [SerializeField] float startSpeed;
    [SerializeField] float distance;
    [SerializeField] Color color;
    [SerializeField] float gap;

    [SerializeField] float preDelayTime;
    [SerializeField] float fireIntervalTime;
    [SerializeField] float postDelayTime;

    enum Color
    {
        blue,
        gray
    }
    public override IEnumerator ExecutePattern()
    {
        boss.IsLeft = target.position.x - bossTransform.position.x <= 0;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Attack3ParameterHash);
        yield return new WaitForSeconds(0.25f);

        Vector3 spawnPos = bossTransform.position + (Vector3.up * 0.5f) + (3 * (boss.IsLeft ? Vector3.left : Vector3.right));
        for (int i = 0; i < AttackCount; ++i)
        {
            for (int j = 0; j < projectileCountPerAttack; ++j)
            {
                float angle = ((360/projectileCountPerAttack)*j + ((360/projectileCountPerAttack)/AttackCount * i)) * Mathf.Deg2Rad;
                
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector3 startPosition = spawnPos + direction.normalized * gap;

                PoolManager.Instance.Get<FoxSphereProjectile>().Rpc_Init(damage, startPosition, preDelayTime, playerRef, startSpeed, distance, (int)color, angle);
            }
            if (EAudioClip != null && EAudioClip.Count > 0)
                SoundManager.Instance.PlaySFX(EAudioClip[0]);
            yield return new WaitForSeconds(fireIntervalTime);
        }

        yield return new WaitForSeconds(preDelayTime);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        yield return new WaitForSeconds(postDelayTime);
    }
}
