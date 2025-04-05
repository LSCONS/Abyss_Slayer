using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTest : MonoBehaviour
{
    //TODO: 테스트 스크립트, 추후 유한상태머신 구현 후 삭제 예정
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }


    private void FixedUpdate()
    {
        Move();
    }


    private Vector2 ComputeMove()
    {
        Vector2 moveVector;
        if(player.input.IsJump && player.playerCheckGround.isGround)
        {
            player.playerRigidbody.AddForce(player.playerData.PlayerAirData.JumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        moveVector = new Vector2(player.input.MoveDir.x * player.playerData.PlayerGroundData.BaseSpeed, player.playerRigidbody.velocity.y);
        return moveVector;
    }


    private void Move()
    {
        player.playerRigidbody.velocity = ComputeMove();
    }
}
