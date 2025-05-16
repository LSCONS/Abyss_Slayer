using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/CrossSlash1")]
public class CrossSlash1Data : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float jumpHight;
    public override IEnumerator ExecutePattern()
    {
        bool isleft = 0 > target.position.x - bossTransform.position.x;
        bossController.isLeft = isleft;
        bossController.StartCoroutine(bossController.RunMove(isleft));
        yield return null;

        while (Mathf.Abs(target.position.x - bossTransform.position.x) > 20f)
        {
            yield return null;
        }
        bossAnimator.SetBool("AttackJump", true);
        Coroutine jump = bossController.StartCoroutine(bossController.JumpMove(target.position,-1,jumpHight));
        
        yield return null;
        bossController.isRun = false;
        yield return new WaitForSeconds(0.2f);

        float time = 0.2f;
        Vector3 dir = new Vector3(isleft ? -0.866f : 0.866f, -0.5f);

        while (!Physics2D.Raycast(bossTransform.position, dir, 12, LayerMask.GetMask("Player")) && time < 1f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        bossAnimator.SetBool("AttackJump", false);
        bossController.StopCoroutine(jump);
        if(time > 1f) yield break;

        bossAnimator.ResetTrigger("Fall");
        PoolManager.Instance.Get<CrossSlash>().Init(bossTransform.position + (4.25f * 1.5f * (isleft ? Vector3.left : Vector3.right)) + (3.6f * Vector3.down), isleft, damage, 1);
        yield return new WaitForSeconds(0.1f);

        bossController.sprite.enabled = false;
        yield return new WaitForSeconds(0.2f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(bossTransform.position, dir, 50, LayerMask.GetMask("GroundPlane", "GroundPlatform"));
        RaycastHit2D hit = hits[0];
        for (int i = 1 ; i < hits.Length; i++)
        {
            float deltaX = Mathf.Abs(hits[i].point.x - bossTransform.position.x);
            if (deltaX >= 7 && (Mathf.Abs(hit.point.x - bossTransform.position.x) < 7f) || Mathf.Abs(hit.point.x - bossTransform.position.x) > deltaX ) 
            {
                hit = hits[i];
            }
        }
        float x = Mathf.Clamp(hit.point.x + (isleft ? 3 : -3),-mapWidth/2 + 0.7f,mapWidth/2+0.7f);
        bossTransform.position = new Vector3(x, hit.point.y + bossCenterHight);
        bossAnimator.SetTrigger("SlashEnd");
        bossController.sprite.enabled = true;
        yield return new WaitForSeconds(1.5f);
        bossAnimator.ResetTrigger("SlashEnd");
    }
}
