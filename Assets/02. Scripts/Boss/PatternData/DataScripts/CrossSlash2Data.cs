using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash2")]
public class CrossSlash2Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float speed = 1;
    [SerializeField] Vector2 scale;
    [SerializeField] float attackDistance;
    [SerializeField] float preDelayTime;
    [SerializeField] float attackdelayTime;
    [SerializeField] float postDelayTime;
    public override IEnumerator ExecutePattern()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        bool isleft = 0 > (target.position.x - bossTransform.position.x);
        boss.IsLeft = isleft;
        bossController.ShowTargetCrosshair = true;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.ReadyRunParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        bossController.StartCoroutine(bossController.RunMove(isleft));
        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        while((Mathf.Abs(target.position.x - bossTransform.position.x) > scale.x + 2))
        {
            yield return null;
        }
        bossController.ShowTargetCrosshair = false;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.RunSlashParameterHash);
        yield return new WaitForSeconds(0.1f * speed);

        bossController.IsRun = false;
        yield return new WaitForSeconds(attackdelayTime);

        if (EAudioClip != null && EAudioClip.Count > 1)
            SoundManager.Instance.PlaySFX(EAudioClip[1]);
        ServerManager.Instance.InitSupporter.Rpc_StartCrossSlashInit(bossTransform.position + 7 * (isleft ? Vector3.left : Vector3.right), isleft, damage, AnimationHash.CrossSlash2ParameterHash, speed, scale.x, scale.y);
        yield return new WaitForSeconds((1/ 6) * speed);

        float x = Mathf.Clamp(bossTransform.position.x + (isleft ? -scale.x : scale.x), -mapWidth / 2 + 0.7f, mapWidth / 2 - 0.7f);
        bossTransform.position = new Vector3(x, bossTransform.position.y);

        if (scene2D.Raycast(bossTransform.position, Vector3.down, bossCenterHight + 0.1f, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask))
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
            yield return new WaitForSeconds(1f);
            boss.Rpc_ResetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
        }
        yield return new WaitForSeconds(postDelayTime);
    }
}
