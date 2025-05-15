using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxClone")]
public class FoxCloneData : BasePatternData
{
    [SerializeField] float hight = 0f;
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] int cloneCount = 4;
    [SerializeField] int cloneDeadDamage = 10;
    [SerializeField] int cloneExplosionDamage = 3;
    [SerializeField] float explosionDelayTime = 8f;
    [SerializeField] float stunTime = 4f;
    [SerializeField] float postDelayTime = 2f;
    float explosionTime;
    int curBossHp;
    int curHp;
    Boss boss;
    List<FoxClone> clones = new List<FoxClone>();
    public override IEnumerator ExecutePattern()
    {
        boss.Rpc_SetAnimationHash(BossAnimationHash.TeleportInParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        int realPosition = UnityEngine.Random.Range(0, cloneCount + 1);

        float width = mapWidth / (cloneCount + 1);
        clones.Clear();
        for (int i = 0; i < cloneCount + 1; i++)
        {
            float positionX = -(mapWidth / 2) + (i * width) + UnityEngine.Random.Range(0, width);
            Vector3 position = new Vector3(positionX, hight + 2);
            
            if(i == realPosition)
            {
                bossTransform.position = position;
                boss.Rpc_SetAnimationHash(BossAnimationHash.TeleportOutParameterHash);
            }
            else
            {
                FoxClone foxClone = PoolManager.Instance.Get<FoxClone>();
                foxClone.Init(position, cloneDeadDamage, cloneExplosionDamage, CloneDead);
                clones.Add(foxClone);
            }
            yield return new WaitForSeconds(0.1f);
        }

        boss = bossController.GetComponentInParent<Boss>();
        curHp = boss.Hp.Value;
        explosionTime = Time.time + explosionDelayTime;
        yield return new WaitUntil(CheckExplosion);
        
        if(clones.Count <= 0)
        {
            boss.Rpc_SetAnimationHash(BossAnimationHash.StunParameterHash);
            yield return new WaitForSeconds(stunTime);
            boss.Rpc_SetAnimationHash(BossAnimationHash.IdleParameterHash);
        }
        else
        {
            boss.Rpc_SetAnimationHash(BossAnimationHash.CloneExplosionParameterHash);

            for(int i = 0 ; i < clones.Count ; i++)
            {
                clones[i].Explosion();
            }

            yield return new WaitForSeconds(1.1f + postDelayTime);
        }

    }
    void CloneDead(FoxClone foxClone)
    {
        clones.Remove(foxClone);
    }
    bool CheckExplosion()
    {
        return (clones.Count == 0) || (Time.time >= explosionTime) || (curHp != boss.Hp.Value);
    }
}
