using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitEffect : BasePoolable
{

    public override void Init()
    {
        gameObject.SetActive(true);
    }

    public override void AutoReturn(float aliveTime)
    {
        StartCoroutine(EnableForAliveTime(aliveTime));
    }

    /// <summary>
    /// 풀 객체인지 확인해서 풀 객체면 풀에 반환, 풀 객체가 아니면 그냥 파괴 (aliveTime 후에)
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableForAliveTime(float aliveTime)
    {
        yield return new WaitForSeconds(aliveTime);

        // 풀 객체인지 확인해서 풀 객체면 풀에 반환, 풀 객체가 아니면 그냥 파괴 (aliveTime 후에)
        var poolable = GetComponent<BasePoolable>();
        if (poolable != null)
        {
            poolable.ReturnToPool();
        }
        else    // 풀 객체가 아니면 그냥 파괴
        {
            Destroy(gameObject);
        }
    }
}
