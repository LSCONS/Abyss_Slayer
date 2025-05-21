using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/MoveDownJump")]
public class MoveDownData : BasePatternData
{
    [SerializeField] float postDelayTime;
    public override IEnumerator ExecutePattern()
    {
        if(bossTransform.position.y <= 5) 
        {
            yield return new WaitForSeconds(0.1f);
            yield break;
        }
        float posY = Physics2D.Raycast(bossTransform.position - Vector3.down * 5, Vector3.down, 20, LayerMask.GetMask("GroundPlane", "GroundPlatform")).point.y + bossCenterHight;
        posY = Random.Range(0, 2f) < 1 ? posY : bossCenterHight;
        yield return bossController.StartCoroutine(bossController.JumpMove(new Vector3(bossTransform.position.x, posY)));
        yield return new WaitForSeconds(postDelayTime);
    }
}
