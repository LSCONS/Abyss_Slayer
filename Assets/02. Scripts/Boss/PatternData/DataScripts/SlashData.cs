using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Slash")]
public class SlashData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float attackSpeed;
    [SerializeField] List<float> attackAngles;
    [SerializeField] float intervalTime;

    [SerializeField] float preDelayTime;
    [SerializeField] float postDelayTime;
    public override IEnumerator ExecutePattern()
    {
        boss.IsLeft = target.position.x - bossTransform.position.x < 0;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashReadyParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        for(int i = 0; i < attackAngles.Count; i++)
        {
            ServerManager.Instance.InitSupporter.Rpc_StartNormalSlashInit(bossTransform.position, damage, boss.IsLeft, attackAngles[i], attackSpeed);
            if (EAudioClip != null && EAudioClip.Count > 0)
                SoundManager.Instance.PlaySFX(EAudioClip[0]);
            bossController.StartCoroutine(AttackEffect(i == attackAngles.Count - 1));
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(2f + postDelayTime);
    }
    IEnumerator AttackEffect(bool lastAttack)
    {
        yield return new WaitForSeconds(0.6f * 1/attackSpeed);
        bossController.Sprite.enabled = false;
        ServerManager.Instance.InitSupporter.Rpc_StartJumpEffectInit(bossTransform.position + Vector3.down * bossCenterHight);
        yield return new WaitForSeconds(0.1f);
        if (lastAttack)
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
        }
        bossController.Sprite.enabled = true;
    }
}
