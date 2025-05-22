using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxSphere")]
public class FoxSphereData : BasePatternData
{
    enum Color
    {
        blue,
        gray
    }

    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] int sphereCount = 3;
    [SerializeField] float fireIntervalTime = 0.3f;
    [SerializeField] float startSpeed = 3;
    [SerializeField] float distance = 10;
    [SerializeField] float postDelayTime = 0.5f;
    [SerializeField] Color color;
    public override IEnumerator ExecutePattern()
    {
        boss.IsLeft = target.position.x - bossTransform.position.x <= 0;
        bossController.ShowTargetCrosshair = true;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Attack3ParameterHash);
        yield return new WaitForSeconds(0.25f);
        Vector3 startPosition = bossTransform.position + (Vector3.up * 0.5f) + (3 * (boss.IsLeft ? Vector3.left : Vector3.right));
        for (int i = 0; i < sphereCount; i++)
        {
            ServerManager.Instance.InitSupporter.Rpc_StartFoxSphereProjectileInit(damage, startPosition, preDelayTime + (i * fireIntervalTime), playerRef, startSpeed, distance, (int)color);
        }

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        yield return new WaitForSeconds(preDelayTime + sphereCount * fireIntervalTime + 0.5f);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        bossController.ShowTargetCrosshair = false;
        yield return new WaitForSeconds(postDelayTime);
    }
}
