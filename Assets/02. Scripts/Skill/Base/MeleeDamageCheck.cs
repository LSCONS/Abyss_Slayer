using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageCheck : MonoBehaviour
{

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// 콜라이더 크기 설정
    /// </summary>
    /// <param name="sizeX"></param>
    /// <param name="sizeY"></param>
    public void SetColliderSize(float sizeX, float sizeY)
    {
        boxCollider.size = new Vector2(sizeX, sizeY);
    }



}
