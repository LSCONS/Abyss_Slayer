using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/HomingMissaile")]
public class HomingMissaileData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] Vector3 startPosition = new Vector3(9f,-3.5f, 0);
    [SerializeField] float startCircleR = 1.5f;
    [SerializeField] float preDelayTime = 1f;
    [SerializeField] int missaileDamage = 10;
    [SerializeField] float missailSpeed = 10;
    [SerializeField] int missaileCount = 8;
    [SerializeField] float fireDegree = 120f;
    [SerializeField] float spreadDegree = 30f;
    [SerializeField] float postDelayTime = 3f;

    public override IEnumerator ExecutePattern(BossController bossController, Transform bossTransform, Animator animator)
    {
        animator.SetTrigger("HomingMissaile1");
        //이동 코드(이동방식에 따라 달라짐
        bossTransform.position = startPosition;
        animator.SetTrigger("HomingMissaile2");
        yield return new WaitForSeconds(preDelayTime);

        for(int i = 0; i < missaileCount; i++)
        {
            float degree = (fireDegree + ((spreadDegree / 2) - (i * spreadDegree / (missaileCount - 1))));
            Quaternion rotate = Quaternion.Euler(0, 0, degree); //오일러 각도로 각도계산

            degree *= Mathf.Deg2Rad;                            //오일러 각도를 라디우스로 변환
            Vector3 position = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree)) * startCircleR;   //라디우스로 위치계산
            position = bossTransform.TransformPoint(position);
            

            PoolManager.Instance.Get<HomingProjectile>().Init(damage, position, rotate, target, missailSpeed, 1.3f - (i * 0.1f));

            yield return new WaitForSeconds(0.1f);
        }
        animator.SetTrigger("HomingMissaile3");
        yield return new WaitForSeconds(postDelayTime);
    }
}
