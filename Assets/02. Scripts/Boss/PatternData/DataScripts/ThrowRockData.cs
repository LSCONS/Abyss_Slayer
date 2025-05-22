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
    [SerializeField] float rockSpeed = 10f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float rockSize = 1f;
    [SerializeField] float postDelayTime = 1f;
    public override IEnumerator ExecutePattern()
    {
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        bossAnimator.SetTrigger("Attack3");
        bossController.ChasingTarget = true;
        bossController.ShowTargetCrosshair = true;
        yield return new WaitForSeconds(preDelayTime + 0.5f);
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        //bossAnimator.SetTrigger("ThrowRock2");
        ServerManager.Instance.InitSupporter.Rpc_StartGravityProjectileInit(damage, bossTransform.position + Vector3.up * spawnPositionY, rockSpeed, playerRef, delayThrowTime, int.MaxValue, rockSize, gravityScale);
        //PoolManager.Instance.Get<GravityProjectile>().Init(damage, bossTransform.position + Vector3.up * 2, rockSpeed, target.position, int.MaxValue,rockSize, 1);
        bossController.ChasingTarget = false;
        bossController.ShowTargetCrosshair = false;
        yield return new WaitForSeconds(1.2f + delayThrowTime);
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        bossAnimator.SetTrigger("Idle");
        yield return new WaitForSeconds(postDelayTime);
    }
}
