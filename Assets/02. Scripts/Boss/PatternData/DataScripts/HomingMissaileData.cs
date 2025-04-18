using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/HomingMissaile")]
public class HomingMissaileData : BasePatternData
{
    [Header("패턴정보")]
    [SerializeField] int damage;
    [SerializeField] Vector3 startPosition = new Vector3(9f, -3.5f, 0);
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] float postDelayTime = 3f;
    [Header("발사정보")]
    [SerializeField] float startCircleR = 1.5f;
    [SerializeField] float fireDegree = 60f;
    [SerializeField] float spreadDegree = 30f;
    [SerializeField] int missaileCount = 8;
    [Header("투사체 정보")]
    [SerializeField] float homingPower = 15f;
    [SerializeField] float missailSpeed = 10;
    [SerializeField] float homingTime = 3f;
    [SerializeField] AnimationCurve homingCurve;
    [SerializeField] AnimationCurve speedCurve;
    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("HomingMissaile1");
        yield return bossController.StartCoroutine(bossController.JumpMove(startPosition));
        bossController.isLeft = true;
        bossAnimator.SetTrigger("HomingMissaile2");
        yield return new WaitForSeconds(preDelayTime);
        bool isLeft = bossController.isLeft;
        for(int i = 0; i < missaileCount; i++)
        {
            float degree = fireDegree - ((spreadDegree / 2) - (i * spreadDegree / (missaileCount - 1)));
            degree = isLeft ? 180 - degree : degree;
            Quaternion rotate = Quaternion.Euler(0, 0, degree); //오일러 각도로 각도계산

            degree *= Mathf.Deg2Rad;                            //오일러 각도를 라디우스로 변환
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;   //라디우스로 위치계산
            position = bossTransform.TransformPoint(position);

            PoolManager.Instance.Get<HomingProjectile>().Init(damage, position, rotate, target, missailSpeed, 1.3f - (i * 0.1f), homingPower, homingTime, homingCurve,speedCurve);

            yield return new WaitForSeconds(0.1f);
        }
        bossAnimator.SetTrigger("HomingMissaile3");
        yield return new WaitForSeconds(postDelayTime);
    }
}
