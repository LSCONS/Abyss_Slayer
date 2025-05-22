using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/RandomMove")]
public class RandomMoveData : BasePatternData
{
    public override IEnumerator ExecutePattern()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        Vector3 targetPos;
        float posX = Random.Range(-mapWidth / 2 + 1, mapWidth / 2 - 1);
        float posY = Random.Range(0, 4) * 5 + 1;
        posY = scene2D.Raycast(new Vector3(posX, posY), Vector3.down, 40, LayerMask.GetMask("GroundPlane", "GroundPlatform")).point.y;
        targetPos = new Vector3(posX, posY + bossCenterHight);

        boss.IsLeft = targetPos.x - bossTransform.position.x < 0;
        if (bossTransform.position.x * targetPos.x < 0)
        {
            bossController.StartCoroutine(bossController.RunMove(boss.IsLeft));
            float i = Mathf.Sign(bossTransform.position.x);
            while (i * Mathf.Sign(bossTransform.position.x) > 0)
            {
                yield return null;
            }
            bossController.IsRun = false;
            yield return null;
        }
        yield return bossController.StartCoroutine(bossController.JumpMove(targetPos));
    }
}
