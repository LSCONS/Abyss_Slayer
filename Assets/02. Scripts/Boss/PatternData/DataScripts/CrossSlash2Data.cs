using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash2")]
public class CrossSlash2Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float speed = 1;
    [SerializeField] Vector2 scale;
    [Tooltip("공격길이 대비 공격시작 거리 비율 0.3~1값")]
    [Range(0.3f, 1f)]
    [SerializeField] float attackDistanceRate = 0.75f;
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

        while((Mathf.Abs(target.position.x - bossTransform.position.x) > (scale.x * attackDistanceRate) + 2f))
        {
            yield return null;
        }
        bossController.ShowTargetCrosshair = false;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.RunSlashParameterHash);
        yield return new WaitForSeconds(0.25f * (1/speed));

        bossController.Sprite.enabled = false;
        bossController.IsRun = false;
        yield return new WaitForSeconds(0.2f * (1/speed));
        Vector3 attackPos = bossTransform.position + (scale.x * 0.5f) * (isleft ? Vector3.left : Vector3.right);
        float x = Mathf.Clamp(bossTransform.position.x + (isleft ? -scale.x : scale.x), -mapWidth / 2 + 0.7f, mapWidth / 2 - 0.7f);
        bossTransform.position = new Vector3(x, bossTransform.position.y);
        bossController.Sprite.enabled = true;

        if (scene2D.Raycast(bossTransform.position, Vector3.down, bossCenterHight + 0.1f, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask))
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
        }
        else
        {
            bossController.StartCoroutine(bossController.Landing());
        }

        if (EAudioClip != null && EAudioClip.Count > 1)
            SoundManager.Instance.PlaySFX(EAudioClip[1]);

        yield return new WaitForSeconds(attackdelayTime);
        ServerManager.Instance.InitSupporter.Rpc_StartCrossSlashInit(attackPos, isleft, damage, AnimationHash.CrossSlash2ParameterHash, speed, scale.x, scale.y);
        yield return new WaitForSeconds((1/ 6) * (1 / speed));

        boss.Rpc_ResetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);


        yield return new WaitForSeconds(postDelayTime);
    }
}
