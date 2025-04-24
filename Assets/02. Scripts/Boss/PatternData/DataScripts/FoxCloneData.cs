using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxCloneData : BasePatternData
{
    [SerializeField] float mapWidth = 40f;
    [SerializeField] float hight = 0f;
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] int cloneCount = 4;
    [SerializeField] int curCloneCount;
    [SerializeField] int cloneDeadDamage = 10;
    [SerializeField] int cloneExplosionDamage = 3;
    [SerializeField] float explosionDelayTime = 8f;
    float explosionTime;
    int curBossHp;
    int curHp;
    Boss boss;
    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("TeleportIn");
        yield return new WaitForSeconds(preDelayTime);

        int realPosition = UnityEngine.Random.Range(0, cloneCount + 1);

        curCloneCount = cloneCount;
        float width = mapWidth / (cloneCount + 1);
        for (int i = 0; i < cloneCount + 1; i++)
        {
            float positionX = -(mapWidth / 2) + (i * width) + UnityEngine.Random.Range(0, width);
            Vector3 position = new Vector3(positionX, hight + 2);
            
            if(i == realPosition)
            {
                bossTransform.position = position;
                bossAnimator.SetTrigger("TeleportOut");
            }
            else
            {
                PoolManager.Instance.Get<FoxClone>().Init(position, cloneDeadDamage, cloneExplosionDamage, CloneDead);
            }
            yield return new WaitForSeconds(0.1f);
        }

        Boss boss = bossController.GetComponent<Boss>();
        curHp = boss.Hp.Value;
        explosionTime = Time.time + explosionDelayTime;
        yield return new WaitUntil(CheckExplosion);
        if(curCloneCount <= 0)
        {
            bossAnimator.SetTrigger("Stun");
        }
    }
    void CloneDead()
    {
        curCloneCount--;
    }
    bool CheckExplosion()
    {
        return (curCloneCount <= 0) || (Time.time >= explosionTime) || (curHp != boss.Hp.Value);
    }
}
