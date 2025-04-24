using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPlayerSilhouette : BasePoolable
{
    [field: SerializeField]private SpriteChange SpriteChange;
    private Color color = new Color(1, 1, 1, 0.3f);
    private WaitForSeconds waitSeconds = new WaitForSeconds(0.16f);

    public override void Init() { }

    public override void Init
        (
            SpriteChange spriteChange,
            AnimationState state, 
            int num, 
            Vector3 position,
            bool flipX
        )
    {
        transform.position = position;
        SpriteChange.SetSpriteCopy(spriteChange);
        SpriteChange.SetFlipXCopy(flipX);
        SpriteChange.SetSpriteColor(color);
        gameObject.SetActive(true);
        StartCoroutine(ReturnPoolObject());
    }


    private IEnumerator ReturnPoolObject()
    {
        yield return waitSeconds;
        ReturnToPool();
    }
}
