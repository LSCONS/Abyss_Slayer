using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    [field: SerializeField]
    public bool CanJump { get;private set; }   //플레이어가 점프가 가능한 상태인지 반환해주는 변수
    public int GroundPlaneCount { get; private set; } = 0;
    public int GroundPlatformCount { get; private set; } = 0;
    public Action playerTriggerOff;     //플레이어의 isTrigger을 off해주는 로직


    //Collider가 땅에 닿을 경우 isGround true반환.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundPlatformLayerIndex)
        {
            CheckAllGroundCanJumpEnter();
            GroundPlatformCount++;
            return;
        }

        if(collision.gameObject.layer == LayerData.GroundPlaneLayerIndex)
        {
            CheckAllGroundCanJumpEnter();
            GroundPlaneCount++;
            return;
        }
    }

    //Collider가 땅에서 빠져나갈 경우 isGround false반환.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundPlatformLayerIndex)
        {
            GroundPlatformCount--;
            CheckAllGroundCanJumpExit();
            return;
        }

        if (collision.gameObject.layer == LayerData.GroundPlaneLayerIndex)
        {
            GroundPlaneCount--;
            CheckAllGroundCanJumpExit();
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GroundPlaneCount = 0;
        GroundPlatformCount = 0;
        playerTriggerOff?.Invoke();
    }

    private void CheckAllGroundCanJumpEnter()
    {
        if(GroundPlaneCount + GroundPlatformCount == 0)
        {
            CanJump = true;
        }
    }

    private void CheckAllGroundCanJumpExit()
    {
        if (GroundPlaneCount + GroundPlatformCount == 0)
        {
            CanJump = false;
        }
    }
}
