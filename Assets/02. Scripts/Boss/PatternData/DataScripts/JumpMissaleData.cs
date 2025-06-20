using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/JumpMissaile")]
public class JumpMissaleData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] int projectileCount;
    [SerializeField] float preDelayTime;
    [SerializeField] List<Vector3> movePositions;
    [SerializeField] float moveableDistance;
    [SerializeField] float jumpDuration;
    [SerializeField] float baseProjectileSpeed;
    [SerializeField] float baseHomingPower;
    [SerializeField] HomingProjectileType projectileType;
    [SerializeField] float postDelayTime;
    [SerializeField] EAniamtionCurve homingCurve;
    [SerializeField] EAniamtionCurve speedCurve;
    public override IEnumerator ExecutePattern()
    {
        bossController.ShowTargetCrosshair = true;
        yield return new WaitForSeconds(preDelayTime);
        Vector3 targetPos;
        float distance;
        do
        {
            targetPos = movePositions[Random.Range(0, movePositions.Count)];
            PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
            targetPos = (Vector3)scene2D.Raycast(targetPos, Vector2.down, 20, LayerMask.GetMask("GroundPlane", "GroundPlatform")).point + Vector3.up * bossCenterHight;
            distance = Vector3.Distance(targetPos, bossTransform.position);
        } while (distance >= moveableDistance || distance <= 5);

        if (EAudioClip != null && EAudioClip.Count > 0)
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[0]);

        bossController.StartCoroutine(bossController.JumpMove(targetPos, jumpDuration));

        yield return new WaitForSeconds(0.2f);

        float intervalTime = (jumpDuration * 0.8f) / projectileCount;
        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 dir = target.position - bossTransform.position;
            Quaternion rot = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x));
            float speed = Random.Range(baseProjectileSpeed * 0.9f, baseProjectileSpeed * 1.1f);
            float homingPow = Random.Range(baseHomingPower * 0.9f, baseHomingPower * 1.1f);
            ManagerHub.Instance.ServerManager.InitSupporter.Rpc_StartHomingProjectileInit(damage, bossTransform.position, rot, playerRef, speed, (int)projectileType, jumpDuration - i * intervalTime, homingPow,0.5f,(int)homingCurve,(int)speedCurve);
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(postDelayTime);
        bossController.ShowTargetCrosshair = false;
    }
}
