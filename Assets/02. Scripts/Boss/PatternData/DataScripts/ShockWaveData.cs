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
        //TODO: 나중에 추가 시 트리거 설정
        bossAnimator.SetTrigger("ShockWave1");
        yield return new WaitForSeconds(preDelayTime);

        //TODO: 나중에 추가 시 트리거 설정
        bossAnimator.SetTrigger("ShockWave2");
        yield return new WaitForSeconds(stompTime);

        Vector3 startPosition = bossTransform.position + Vector3.down * bossCenterHight;

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        ServerManager.Instance.InitSupporter.Rpc_StartExplosionInit(startPosition + (Vector3.up * 0.3f), damage, 0.5f);
        //PoolManager.Instance.Get<Explosion>().Init(startPosition + (Vector3.up * 0.3f), damage, 0.5f);
        bossController.StartCoroutine(ShockWave(startPosition));

        yield return new WaitForSeconds(postDelayTime);
    }
    IEnumerator ShockWave(Vector3 startPosition)
    {
        for (int i = 0; i < shockCount; i++)
        {
            ServerManager.Instance.InitSupporter.Rpc_StartShockWaveInit(startPosition + (Vector3.right * shockOffsetWidth * (i + 1)), damage);
            ServerManager.Instance.InitSupporter.Rpc_StartShockWaveInit(startPosition + (Vector3.left * shockOffsetWidth * (i + 1)), damage);
            //PoolManager.Instance.Get<ShockWave>().Init(startPosition + (Vector3.right * shockOffsetWidth * (i + 1)), damage);
            //PoolManager.Instance.Get<ShockWave>().Init(startPosition + (Vector3.left * shockOffsetWidth * (i + 1)), damage);

            yield return new WaitForSeconds(shockOffsetTime);
        }
    }
}
