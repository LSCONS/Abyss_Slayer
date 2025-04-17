using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


[CreateAssetMenu(fileName = "DashAttackSkill", menuName = "SkillRefactory/Melee/DashAttackSkill")]
public class DashAttackSkill : MeleeAttackSkill
{
   public float dashSpeed = 10f;
   public float dashDuration = 0.5f;
   public float dashDistance = 5f;// 대시 거리
   public float colliderDuration = 0.5f;
   

   public override void UseSkill()
   {
        // 대시 방향 설정
        Vector2 dashDirection = player.input.MoveDir.normalized;
        if(dashDirection == Vector2.zero)
            dashDirection = new Vector2(PlayerFrontXNomalized(), 0);

        if(dashDirection == Vector2.zero)
            return;

        // 스프라이트 방향 설정
        if(dashDirection.x > 0) player.SpriteRenderer.flipX = false;
        else if (dashDirection.x < 0) player.SpriteRenderer.flipX = true;

        // 플레이어 출발 위치 저장
        Vector3 startPos = player.transform.position;

        // 플레이어의 리지드 바디로 대시
        player.playerRigidbody.velocity = Vector2.zero; // 대시 전 속도 초기화
        player.playerRigidbody.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);

        // 대시 이후의 처리들 (데미지 넣을 콜라이더 이동 + )
        player.StartCoroutine(DashCoroutine(startPos, dashDuration));

   }

   private IEnumerator DashCoroutine(Vector2 startPos, float dashDuration)
   {
        // 대시 이후의 처리들 (데미지 넣을 콜라이더 이동 + )
        yield return new WaitForSeconds(dashDuration);  // dashDuration 만큼 대기하고 콜리전 이동시킬거임

        Vector2 endPos = player.transform.position;
        Vector2 midPos = (startPos + endPos) / 2;

        // 콜라이더 위치 저장 이동
        Vector2 originalPos = player.playerMeleeCollider.transform.localPosition;
        player.playerMeleeCollider.transform.position = midPos;

        // 콜라이더 켜
        player.OnMeleeAttack?.Invoke(player.playerMeleeCollider, dashDuration);     // 콜라이더? 알아서 꺼져

        // 콜라이더 위치 원래대로 돌리기
        yield return new WaitForSeconds(colliderDuration);
        player.playerMeleeCollider.transform.position = originalPos;


    }
}
