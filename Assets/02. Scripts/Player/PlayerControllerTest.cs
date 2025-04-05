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


    /// <summary>
    /// 입력받은 곳으로 특정 Speed값으로 이동하거나 Jump를 한꺼번에 계산해서 Vector2로 반환해주는 임시 메서드
    /// </summary>
    /// <returns>현재 이동할 velocity의 값을 Vector2로 반환</returns>
    private Vector2 ComputeMove()
    {
        Vector2 moveVector;
        if(player.input.IsJump && player.playerCheckGround.CanJump)
        {
            player.playerRigidbody.AddForce(player.playerData.PlayerAirData.JumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        moveVector = new Vector2(player.input.MoveDir.x * player.playerData.PlayerGroundData.BaseSpeed, player.playerRigidbody.velocity.y);
        return moveVector;
    }


    /// <summary>
    /// 플레이어의 이동이나 점프를 실행시킬 메서드
    /// </summary>
    private void Move()
    {
        player.playerRigidbody.velocity = ComputeMove();
    }
}
