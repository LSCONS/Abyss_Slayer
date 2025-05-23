using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash1")]
public class CrossSlash1Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float jumpHight;
    [SerializeField] float preDelayTime;
    [SerializeField] float attackDelayTime;
    [SerializeField] float postDelayTime;
    [SerializeField] float jumpSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] Vector2 scale;
    public override IEnumerator ExecutePattern()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        bool isleft = 0 > target.position.x - bossTransform.position.x;
        boss.IsLeft = isleft;
        bossController.ShowTargetCrosshair = true;

        if (EAudioClip != null && EAudioClip.Count > 0)
            SoundManager.Instance.PlaySFX(EAudioClip[0]);

        boss.Rpc_SetTriggerAnimationHash(AnimationHash.ReadyRunParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        bossController.StartCoroutine(bossController.RunMove(isleft));
        yield return null;

        while (Mathf.Abs(target.position.x - bossTransform.position.x) > scale.x)
        {
            yield return null;
        }
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, true);
        Coroutine jump = bossController.StartCoroutine(bossController.JumpMove(target.position,1/jumpSpeed,jumpHight));
        bossController.ShowTargetCrosshair = false;

        yield return null;
        bossController.IsRun = false;
        yield return new WaitForSeconds(0.2f * (1 / jumpSpeed));

        float time = 0.2f * (1 / jumpSpeed);
        Vector3 dir = new Vector3(isleft ? -scale.x : scale.x, -scale.y);

        while (!scene2D.Raycast(bossTransform.position, dir, 12, LayerData.PlayerLayerMask) && time < 1 / jumpSpeed)
        {
            time += Time.deltaTime;
            yield return null;
        }
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, false);
        
        bossController.StopCoroutine(jump);
        if(time > 1f) yield break;

        boss.Rpc_ResetTriggerAnimationHash(AnimationHash.FallParameterHash);
        ServerManager.Instance.InitSupporter.Rpc_StartCrossSlashInit(bossTransform.position + (4.25f * 1.5f * (isleft ? Vector3.left : Vector3.right)) + (3.6f * Vector3.down), isleft, damage, AnimationHash.CrossSlash1ParameterHash,attackSpeed,scale.x,scale.y);    
        yield return new WaitForSeconds(0.1f);

        bossController.Sprite.enabled = false;
        yield return new WaitForSeconds(0.2f);

        List<RaycastHit2D> hits = new();
        var filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask;

        int hitcount = scene2D.Raycast
            (
            origin: bossTransform.position,
            direction: dir,
            contactFilter : filter,
            results: hits,
            distance: 50f
            );

        if (EAudioClip != null && EAudioClip.Count > 1)
            SoundManager.Instance.PlaySFX(EAudioClip[1]);

        RaycastHit2D hit = hits[0];
        for (int i = 1 ; i < hits.Count; i++)
        {
            float deltaX = Mathf.Abs(hits[i].point.x - bossTransform.position.x);
            if (deltaX >= 7 && (Mathf.Abs(hit.point.x - bossTransform.position.x) < 7f) || Mathf.Abs(hit.point.x - bossTransform.position.x) > deltaX ) 
            {
                hit = hits[i];
            }
        }
        float x = Mathf.Clamp(hit.point.x + (isleft ? 3 : -3),-mapWidth/2 + 0.7f,mapWidth/2+0.7f);
        bossTransform.position = new Vector3(x, hit.point.y + bossCenterHight);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
        bossController.Sprite.enabled = true;
        yield return new WaitForSeconds(1.5f);
        boss.Rpc_ResetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
    }
}
