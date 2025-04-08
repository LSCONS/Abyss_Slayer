using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    [field: SerializeField]
    public bool CanJump { get;private set; }   //플레이어가 점프가 가능한 상태인지 반환해주는 변수
    private int nowGroundCount = 0;

    //Collider가 땅에 닿을 경우 isGround true반환.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            if (nowGroundCount == 0)
            {
                CanJump = true;
            }
            nowGroundCount++;
        }
    }

    //Collider가 땅에서 빠져나갈 경우 isGround false반환.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            nowGroundCount--;
            if (nowGroundCount == 0)
            {
                CanJump = false;
            }
        }
    }
}
