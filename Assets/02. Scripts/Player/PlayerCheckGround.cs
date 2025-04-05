using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    public bool isGround = false;   //플레이어가 땅을 확인하는 Collider로 땅에 닿았는지 체크해주는 변수

    //Collider가 땅에 닿을 경우 isGround true반환.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            isGround = true;
        }
    }

    //Collider가 땅에서 빠져나갈 경우 isGround false반환.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            isGround = false;
        }
    }
}
