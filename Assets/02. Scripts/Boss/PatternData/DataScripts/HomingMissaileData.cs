using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/HomingMissaile")]
public class HomingMissaileData : BasePatternData
{
    
    [Header("패턴정보")]
    [SerializeField] int damage;
    [SerializeField] List<Vector3> startPositions = new List<Vector3>();
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] float postDelayTime = 3f;
    [SerializeField] bool isMultyTarget;
    [Header("발사정보")]
    [SerializeField] float startCircleR = 1.5f;
    [SerializeField] float fireDegree = 60f;
    [SerializeField] float spreadDegree = 30f;
    [SerializeField] int missaileCount = 8;
    [Header("투사체 정보")]
    [SerializeField] float homingPower = 15f;
    [SerializeField] float randomSpeedRate;
    [SerializeField] float missailSpeed = 10;
    [SerializeField] float explosionSize = 0.5f;
    [SerializeField] EAniamtionCurve homingCurve;
    [SerializeField] EAniamtionCurve speedCurve;
    [SerializeField] HomingProjectileType projectileType;
    public override IEnumerator ExecutePattern()
    {
        if(startPositions.Count > 0)
        {
            Vector3 startPosition;
            startPosition = startPositions[Random.Range(0, startPositions.Count)];
            yield return bossController.StartCoroutine(bossController.JumpMove(startPosition));
        }
        boss.IsLeft = bossTransform.position.x > 0;
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.Attack2ParameterHash);
        yield return new WaitForSeconds(preDelayTime/2);
        bool isLeft = boss.IsLeft;
        for (int i = 0; i < missaileCount; i++)
        {
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (missaileCount - 1)));
            degree = isLeft ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree); //오일러 각도로 각도계산

            degree *= Mathf.Deg2Rad;                            //오일러 각도를 라디우스로 변환
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;   //라디우스로 위치계산
            position = bossTransform.TransformPoint(position);

            if (isMultyTarget)
            {
                Player[] players = ServerManager.Instance.DictRefToPlayer.Values.ToArray();
                target = players[Random.Range(0, players.Length)].transform;
            }
            ServerManager.Instance.InitSupporter.Rpc_StartHomingProjectileInit(damage, position, rotate, playerRef, missailSpeed * Random.Range(1,(100 + randomSpeedRate)/100f), (int)projectileType, (preDelayTime / 2 + 0.1f * missaileCount) - (i * 0.1f), homingPower, explosionSize, (int)homingCurve, (int)speedCurve);
            //PoolManager.Instance.Get<HomingProjectile>().Init(damage, position, rotate, target, missailSpeed, (preDelayTime / 2 + 0.1f * missaileCount) - (i * 0.1f), homingPower, homingTime, explosionSize, homingCurve,speedCurve);

            if (EAudioClip != null && EAudioClip.Count > 0)
                SoundManager.Instance.PlaySFX(EAudioClip[0]);

            yield return new WaitForSeconds(0.1f);
        }
        //TODO: 나중에 애니메이션 트리거 추가 시 Rpc 추가
        bossAnimator.SetTrigger("HomingMissaile3");
        yield return new WaitForSeconds(postDelayTime);
    }
}
