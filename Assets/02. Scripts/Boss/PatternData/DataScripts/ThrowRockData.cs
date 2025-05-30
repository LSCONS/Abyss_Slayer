using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/ThrowRock")]
public class ThrowRockData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 0f;
    [SerializeField] float delayThrowTime = 0f;
    [SerializeField] float spawnPositionY = 3f;
    [SerializeField] float rockSpeed = 1f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float minSpeed;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float rockSize = 1f;
    [SerializeField] float postDelayTime = 1f;
    public override IEnumerator ExecutePattern()
    {
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Attack3ParameterHash);
        bossController.ChasingTarget = true;
        bossController.ShowTargetCrosshair = true;
        yield return new WaitForSeconds(preDelayTime + 0.5f);

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        ServerManager.Instance.InitSupporter.Rpc_StartGravityProjectileInit(damage, bossTransform.position + Vector3.up * spawnPositionY, rockSpeed, maxSpeed, minSpeed, playerRef, delayThrowTime, int.MaxValue, rockSize, gravityScale);
        
        yield return new WaitForSeconds(1.2f + delayThrowTime);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        yield return new WaitForSeconds(postDelayTime);
        bossController.ChasingTarget = false;
        bossController.ShowTargetCrosshair = false;
    }
}
