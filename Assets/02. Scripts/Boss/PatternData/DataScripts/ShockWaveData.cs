using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/ShockWave")]
public class ShockWaveData : BasePatternData
{
    [Header("개별 패턴 세부 정보")]
    [SerializeField] int damage;
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] float stompTime = 0.2f;
    [SerializeField] int shockCount = 6;
    [SerializeField] float shockOffsetWidth = 1.0f;
    [SerializeField] float shockOffsetTime = 0.15f;
    [SerializeField] float postDelayTime = 3f;
    public override IEnumerator ExecutePattern()
    {
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.ShockWave1ParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        boss.Rpc_SetTriggerAnimationHash(AnimationHash.ShockWave2ParameterHash);
        yield return new WaitForSeconds(stompTime);

        Vector3 startPosition = bossTransform.position + Vector3.down * bossCenterHight;

        if (EAudioClip != null && EAudioClip.Count > 0)
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[0]);

        ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartExplosionInit(startPosition + (Vector3.up * 0.3f), damage, 0.5f);
        //PoolManager.Instance.Get<Explosion>().Init(startPosition + (Vector3.up * 0.3f), damage, 0.5f);
        bossController.StartCoroutine(ShockWave(startPosition));

        yield return new WaitForSeconds(postDelayTime);
    }
    IEnumerator ShockWave(Vector3 startPosition)
    {
        for (int i = 0; i < shockCount; i++)
        {
            ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartShockWaveInit(startPosition + (Vector3.right * shockOffsetWidth * (i + 1)), damage);
            ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartShockWaveInit(startPosition + (Vector3.left * shockOffsetWidth * (i + 1)), damage);

            yield return new WaitForSeconds(shockOffsetTime);
        }
    }
}
