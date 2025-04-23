using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxCloneData : BasePatternData
{
    [SerializeField] float preDelayTime = 2f;
    [SerializeField] int cloneCount = 4;


    public override IEnumerator ExecutePattern()
    {
        bossAnimator.SetTrigger("FoxClone1");
        yield return new WaitForSeconds(preDelayTime);


    }
}
