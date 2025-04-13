using System;
using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    [field: SerializeField]
    public bool CanJump { get;private set; }   //플레이어가 점프가 가능한 상태인지 반환해주는 변수
    public int GroundPlaneCount { get; private set; } = 0;
    public int GroundPlatformCount { get; private set; } = 0;
    public Action playerTriggerOff;     //플레이어의 isTrigger을 off해주는 로직


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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundPlaneLayerIndex)
        {
            playerTriggerOff?.Invoke();
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ResetAllGround();
        playerTriggerOff?.Invoke();
    }


    public void ResetAllGround()
    {
        GroundPlaneCount = 0;
        GroundPlatformCount = 0;
    }

    /// <summary>
    /// 아직까지 닿은 바닥이 하나도 없었다면 점프가 가능해짐.
    /// </summary>
    private void CheckAllGroundCanJumpEnter()
    {
        if(GroundPlaneCount + GroundPlatformCount == 0)
        {
            CanJump = true;
        }
    }


    /// <summary>
    /// 닿은 바닥에서 떨어졌을 때 닿고있는 바닥이 하나도 없다면 점프기 불가능해짐.
    /// </summary>
    private void CheckAllGroundCanJumpExit()
    {
        if (GroundPlaneCount + GroundPlatformCount == 0)
        {
            CanJump = false;
        }
    }
}
