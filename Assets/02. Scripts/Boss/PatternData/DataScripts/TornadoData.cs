using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/Tornado")]
public class TornadoData : BasePatternData
{
    [Header("개별 패턴 세부 정보")]
    [SerializeField] int damage;
    [SerializeField] float attackPerSec = 5f;
    [SerializeField] float preDelayTime = 0.5f;
    [Tooltip("토네이도가 생성될 위치 (월드 x좌표값) 디자인상 양끝2개 기본으로 생각하고 만듦")]
    [SerializeField] List<float> spawnPositionsX = new List<float> { 10, -10 };
    [SerializeField] float groundPositionY = -5f;
    [SerializeField] float tornadoWidth = 1f;
    [SerializeField] float warningTime = 1f;
    [SerializeField] float durationTime = 2.5f;
    [SerializeField] float postDelayTime = 1f;

    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("Tornado1");                    //공격모션 애니메이션 삽입                              
        yield return new WaitForSeconds(preDelayTime);      //공격모션 선딜

        bossAnimator.SetTrigger("Tornado2");                    //공격 애니메이션 삽입

        //자신위치에 토네이도 한개 생성
        PoolManager.Instance.Get<Tornado>().Init(new Vector3(bossTransform.position.x, groundPositionY), damage, durationTime, attackPerSec, warningTime, tornadoWidth);

        //지정된 위치(기본 맵양끝)에 전부 토네이도 생성
        for (int i = 0; i < spawnPositionsX.Count; i++)
        {
            PoolManager.Instance.Get<Tornado>().Init(new Vector3(spawnPositionsX[i], groundPositionY), damage, durationTime, attackPerSec, warningTime, tornadoWidth);
        }

        yield return new WaitForSeconds(warningTime + durationTime);
        bossAnimator.SetTrigger("Tornado3");
        yield return new WaitForSeconds(postDelayTime);
    }

}
