using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxSphereData : BasePatternData
{
    [SerializeField] int damage;
    [SerializeField] float preDelayTime;
    public override IEnumerator ExecutePattern()
    {
        yield return new WaitForSeconds(preDelayTime);
    }
}
