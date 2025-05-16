using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float missailSpeed = 10;
    [SerializeField] float homingTime = 3f;
    [SerializeField] float explosionSize = 0.5f;
    [SerializeField] AnimationCurve homingCurve;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] HomingProjectileType projectileType;
    public override IEnumerator ExecutePattern()
    {
        if(startPositions.Count > 0)
        {
            Vector3 startPosition;
            startPosition = startPositions[Random.Range(0, startPositions.Count)];
            yield return bossController.StartCoroutine(bossController.JumpMove(startPosition));
        }
        bossController.isLeft = bossTransform.position.x > 0;
        bossAnimator.SetTrigger("Attack2");
        yield return new WaitForSeconds(preDelayTime/2);
        bool isLeft = bossController.isLeft;
        for(int i = 0; i < missaileCount; i++)
        {
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (missaileCount - 1)));
            degree = isLeft ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree); //오일러 각도로 각도계산

            degree *= Mathf.Deg2Rad;                            //오일러 각도를 라디우스로 변환
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;   //라디우스로 위치계산
            position = bossTransform.TransformPoint(position);

            if (isMultyTarget)
            {
                Collider2D[] players = Physics2D.OverlapAreaAll(new Vector2(-mapWidth / 2, 0), new Vector2(mapWidth / 2, 20), LayerMask.GetMask("Player"));
                target = players[Random.Range(0, players.Length)].transform;
            }
            PoolManager.Instance.Get<HomingProjectile>().Init(damage, position, rotate, target, missailSpeed, projectileType,(preDelayTime / 2 + 0.1f * missaileCount) - (i * 0.1f), homingPower * Random.Range(0.8f,1.2f), homingTime, explosionSize, homingCurve,speedCurve);

            yield return new WaitForSeconds(0.1f);
        }
        bossAnimator.SetTrigger("HomingMissaile3");
        yield return new WaitForSeconds(postDelayTime);
    }
}
