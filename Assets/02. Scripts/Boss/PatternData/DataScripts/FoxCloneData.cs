using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern/FoxClone")]
public class FoxCloneData : BasePatternData
{
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] int cloneCount = 4;
    [SerializeField] int cloneDeadDamage = 10;
    [SerializeField] int cloneExplosionDamage = 3;
    [SerializeField] float explosionDelayTime = 8f;
    [SerializeField] float stunTime = 4f;
    [SerializeField] float postDelayTime = 2f;
    [SerializeField] int cloneHp;
    float explosionTime;
    int curHp;
    public override IEnumerator ExecutePattern()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        boss.Rpc_SetTriggerAnimationHash(AnimationHash.TeleportInParameterHash);
        yield return new WaitForSeconds(preDelayTime);

        int realPosition = Random.Range(0, cloneCount + 1);

        float width = mapWidth / (cloneCount + 1);
        boss.Clones.Clear();
        for (int i = 0; i < cloneCount + 1; i++)
        {
            float positionX = -(mapWidth / 2) + (i * width) + Random.Range(0, width);
            float positionY = scene2D.Raycast(new Vector3(positionX, 5 * Random.Range(0, 3) + 1), Vector3.down, 20, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask).point.y;
            Vector3 position = new Vector3(positionX, positionY + bossCenterHight);
            
            if(i == realPosition)
            {
                bossTransform.position = position;
                boss.Rpc_SetTriggerAnimationHash(AnimationHash.TeleportOutParameterHash);
            }
            else
            { 
                FoxClone foxClone = PoolManager.Instance.Get<FoxClone>();
                foxClone.Rpc_Init(position, cloneDeadDamage, cloneExplosionDamage, 1, cloneHp);
                boss.Clones.Add(foxClone);
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (EAudioClip != null && EAudioClip.Count > 0)
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[0]);

        boss = bossController.GetComponentInParent<Boss>();
        curHp = boss.Hp.Value;
        explosionTime = Time.time + explosionDelayTime;
        yield return new WaitUntil(CheckExplosion);
        
        if(boss.Clones.Count <= 0)
        {
            if (EAudioClip != null && EAudioClip.Count > 1)
                ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[1]);

            boss.Rpc_SetTriggerAnimationHash(AnimationHash.StunParameterHash);
            yield return new WaitForSeconds(stunTime);
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.IdleParameterHash);
        }
        else
        {
            boss.Rpc_SetTriggerAnimationHash(AnimationHash.CloneExplosionParameterHash);

            for(int i = 0 ; i < boss.Clones.Count ; i++)
            {
                boss.Clones[i].Rpc_Explosion();
            }

            if (EAudioClip != null && EAudioClip.Count > 2)
                ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip[2]);


            yield return new WaitForSeconds(1.1f + postDelayTime);
        }

    }
    bool CheckExplosion()
    {
        return (boss.Clones.Count == 0) || (Time.time >= explosionTime) || (curHp != boss.Hp.Value);
    }
}
