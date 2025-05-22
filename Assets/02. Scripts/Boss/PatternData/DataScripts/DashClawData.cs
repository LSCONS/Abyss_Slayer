using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/DashClaw")]
public class DashClawData : BasePatternData
{
    
    [SerializeField] int damage;
    [SerializeField] float distance = 15f;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] float attackDuration = 1f;
    [SerializeField] int comboAttackCount = 3;
    [SerializeField] float comboDelayTime = 0.5f;
    [SerializeField] float postDelayTime = 2f;
    public override IEnumerator ExecutePattern()
    {
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Dash1ParameterHash);

        bossController.ShowTargetCrosshair = true;

        bool isLeft = target.transform.position.x - bossTransform.position.x < 0;
        boss.IsLeft = isLeft;
        ServerManager.Instance.InitSupporter.Rpc_StartDashClawEffectInit(damage, bossTransform.position + 2 * (isLeft ? Vector3.right : Vector3.left), isLeft, distance, preDelayTime, attackDuration);
        bossController.ChasingTarget = false;

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        yield return new WaitForSeconds(preDelayTime);
        bossAnimator.enabled = false;
        bossController.Sprite.enabled = false;
        bossController.HitCollider.enabled = false;

        yield return new WaitForSeconds(0.3f * attackDuration);
        float positionX = Mathf.Clamp(bossController.transform.position.x + (boss.IsLeft ? -distance : distance), -mapWidth / 2 + 1, mapWidth / 2 - 1);
        bossTransform.position = new Vector3(positionX,bossTransform.position.y,0);
        bossController.Sprite.enabled = true;
        bossController.HitCollider.enabled = true;

        int i = 1;
        while(i < comboAttackCount)
        {
            isLeft = target.transform.position.x - bossTransform.position.x < 0;
            ServerManager.Instance.InitSupporter.Rpc_StartDashClawEffectInit(damage, bossTransform.position + 2 * (isLeft ? Vector3.right : Vector3.left), isLeft, distance, comboDelayTime, attackDuration);

            if (EAudioClip != null && EAudioClip.Count > 1)
                SoundManager.Instance.PlaySFX(EAudioClip[1]);

            yield return new WaitForSeconds(comboDelayTime);
            bossController.Sprite.enabled = false;
            bossController.HitCollider.enabled = false;
            boss.IsLeft = isLeft;

            yield return new WaitForSeconds(0.3f * attackDuration);
            positionX = Mathf.Clamp(bossController.transform.position.x + (boss.IsLeft ? -distance : distance), -mapWidth / 2 + 1, mapWidth / 2 - 1);
            bossTransform.position = new Vector3(positionX, bossTransform.position.y, 0);
            bossController.Sprite.enabled = true;
            bossController.HitCollider.enabled = true;
            i++;
        }
        bossAnimator.enabled = true;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Dash3ParameterHash);
        bossController.ShowTargetCrosshair = false;
        float time = Time.time + (postDelayTime/2);
        Vector3 targetposition = bossTransform.position + 2f * (isLeft ? Vector3.left : Vector3.right);
        targetposition = new Vector3(Mathf.Clamp(targetposition.x, -mapWidth / 2 + 1, mapWidth / 2 - 1), targetposition.y, targetposition.z);
        while(time >= Time.time)
        {
            bossTransform.position = Vector3.Lerp(bossTransform.position, targetposition, 2 * 0.9f * Time.deltaTime);
            yield return null;
        }
        yield return bossController.StartCoroutine(bossController.Landing());
        yield return new WaitForSeconds(postDelayTime/2);
    }
}
