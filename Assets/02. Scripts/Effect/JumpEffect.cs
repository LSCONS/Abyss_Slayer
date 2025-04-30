using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : BasePoolable
{
    public override void Init()
    {
    }
    public void Init(Vector3 position, float size = 1f)
    {
        transform.position = position;
        transform.localScale = Vector3.one * size;
    }
}
