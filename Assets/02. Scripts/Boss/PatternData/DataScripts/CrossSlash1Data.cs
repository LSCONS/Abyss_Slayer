using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash1")]
public class CrossSlash1Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float addJumpHight;
    [SerializeField] float preDelayTime;
    [SerializeField] float attackDelayTime;
    [SerializeField] float postDelayTime;
    [SerializeField] float jumpDistanceX;
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

        while (Mathf.Abs(target.position.x - bossTransform.position.x) > scale.x/2 + jumpDistanceX)
        {
            yield return null;
        }
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, true);
        float jumpHight = 0.6f * scale.y + addJumpHight;
        Vector3 jumpPos = new Vector3(bossTransform.position.x + (isleft ? - jumpDistanceX : jumpDistanceX), target.position.y + jumpHight);
        Coroutine jump = bossController.StartCoroutine(bossController.JumpMove(jumpPos,1/jumpSpeed,0));
        bossController.ShowTargetCrosshair = false;

        yield return null;
        bossController.IsRun = false;

        yield return new WaitForSeconds(1/jumpSpeed);
        boss.Rpc_SetBoolAnimationHash(AnimationHash.AttackJumpParameterHash, false);
        
        boss.Rpc_ResetTriggerAnimationHash(AnimationHash.FallParameterHash);
        Vector3 attackPos = bossTransform.position + (scale.x * 0.5f * (isleft ? Vector3.left : Vector3.right)) + (scale.y * 0.5f * Vector3.down);
        
        yield return new WaitForSeconds(0.1f);
        bossController.Sprite.enabled = false;
        yield return new WaitForSeconds(0.2f);

        
        if (EAudioClip != null && EAudioClip.Count > 1)
            SoundManager.Instance.PlaySFX(EAudioClip[1]);

        float x = Mathf.Clamp(bossTransform.position.x + (scale.x + 2) * (isleft ? -1 : 1), -mapWidth/2 + 0.7f,mapWidth/2 - 0.7f);
        float y = scene2D.Raycast(new Vector3(x, bossTransform.position.y - scale.y, 0), Vector3.down, 20, LayerMask.GetMask("GroundPlane", "GroundPlatform")).point.y + bossCenterHight;
        bossTransform.position = new Vector3(x, y);
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
        bossController.Sprite.enabled = true;

        yield return new WaitForSeconds(attackDelayTime);
        ServerManager.Instance.InitSupporter.Rpc_StartCrossSlashInit(attackPos, isleft, damage, AnimationHash.CrossSlash1ParameterHash, attackSpeed, scale.x, scale.y);
        
        yield return new WaitForSeconds(1 + postDelayTime);
        boss.Rpc_ResetTriggerAnimationHash(AnimationHash.SlashEndParameterHash);
    }
}
