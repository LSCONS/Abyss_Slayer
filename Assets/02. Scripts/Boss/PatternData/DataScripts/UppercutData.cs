using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Uppercut")]
public class UppercutData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float attackPerSec = 5f;
    [SerializeField] float accessSpeed = 10f;
    [SerializeField] float preDelayTime = 0.8f;
    [SerializeField] float warningTime = 0.4f;
    [SerializeField] float duration = 0.2f;
    [SerializeField] float width = 0.8f;
    [SerializeField] float jumpTime = 1f;
    [SerializeField] float jumpHight = 3f;
    [SerializeField] float postDelayTime = 1f;
    public override IEnumerator ExecutePattern()
    {
        Vector3 targetPosition = target.position;
        bossController.ChasingTarget = true;
        yield return new WaitForSeconds(preDelayTime);
        bossController.ShowTargetCrosshair = true;
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Uppercut1ParameterHash);
        yield return new WaitForSeconds(0.2f);
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Uppercut2ParameterHash);

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        while (Mathf.Abs(targetPosition.x - bossTransform.position.x) >= 0.05f)
        {
            float x = Mathf.Lerp(bossTransform.position.x, targetPosition.x, accessSpeed * Time.deltaTime);
            bossTransform.position = new Vector3(x, bossTransform.position.y, 0);
            yield return null;
        }
        bossController.ChasingTarget = false;
        bossController.ShowTargetCrosshair = false;
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Uppercut3ParameterHash);
        ServerManager.Instance.InitSupporter.Rpc_StartTornadoInit(bossTransform.position + Vector3.down * bossCenterHight, damage, duration, attackPerSec, warningTime, width);
        //PoolManager.Instance.Get<Tornado>().Init(bossTransform.position + Vector3.down * bossCenterHight, damage, duration, attackPerSec, warningTime, width);
        yield return new WaitForSeconds(warningTime);
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Uppercut4ParameterHash);
        yield return bossController.StartCoroutine(bossController.JumpMove(targetPosition + Vector3.up * jumpHight, jumpTime, 0f));
        yield return bossController.StartCoroutine(bossController.Landing());
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Uppercut5ParameterHash);
        yield return new WaitForSeconds(postDelayTime);
    }
}
