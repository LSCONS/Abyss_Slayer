using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.PlayerLoop;


[CreateAssetMenu(fileName = "DashAttackSkill", menuName = "SkillRefactory/Melee/DashAttackSkill")]
public class DashAttackSkill : MeleeAttackSkill
{
    public float damage = 10f;
    public float dashDuration = 0.5f;
    public float dashDistance = 5f;
    public float colliderDuration = 0.5f;

    public float skillDuration = 1f;    // 스킬 지속 시간

    public GameObject dashColliderPrefab;
    public GameObject dashEffectPrefab;
    public GameObject dashAttackEffectPrefab;



    public override void UseSkill()
   {
        // 대시 방향 설정
        Vector2 dashDirection = player.SpriteRenderer.flipX ? Vector2.left : Vector2.right;

        // 대시 이후의 처리들 (데미지 넣을 콜라이더 이동 + )
        player.StartCoroutine(DashCoroutine(dashDirection, dashDuration));
   }

   private IEnumerator DashCoroutine(Vector2 dashDirection, float dashDuration)
   {
        // 시작 위치 타겟위치 설정
        float time = 0;
        Vector2 startPos = player.transform.position;
        Vector2 targetPos = startPos + dashDirection * dashDistance;

        // 대쉬 이펙트 생성
        GameObject dashEffect = null;
        if (dashEffectPrefab != null)
        {
            // 위치 설정
            Vector3 effectOffset = new Vector3(player.SpriteRenderer.flipX ? 1f : -1f, -0.5f, 0);   // 플립되어있으면 1f, 아니면 -1f
            Vector3 effectPos = player.transform.position + effectOffset;

            dashEffect = GameObject.Instantiate(dashEffectPrefab, effectPos, Quaternion.identity, player.transform);
            dashEffect.transform.right = dashDirection;

            // 이펙트 방향 설정
            SpriteRenderer effectSpriteRenderer = dashEffect.GetComponent<SpriteRenderer>();
            if(effectSpriteRenderer != null)
            {
                effectSpriteRenderer.flipX = dashDirection.x < 0;
            }
        }


        // dashDistance만큼을 dashDuration 시간동안 이동
        while(time < dashDuration)
        {
            player.playerRigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, time / dashDuration));
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 대시 이후 위치
        player.playerRigidbody.MovePosition(targetPos);

        // 이펙트 제거
        if(dashEffect != null)
        {
            GameObject.Destroy(dashEffect, 0.5f);
        }

        // 끝/중간 위치 설정
        Vector2 endPos = player.transform.position;
        Vector2 midPos = (startPos + endPos) / 2;
        float distance = Vector2.Distance(startPos, endPos);

        // 콜라이더 위치 저장 이동
        Vector2 originalPos = player.playerMeleeCollider.transform.position;
        player.playerMeleeCollider.transform.position = midPos;

        // 콜라이더 세팅
        GameObject colliderObj = GameObject.Instantiate(dashColliderPrefab, midPos, Quaternion.identity);
        MeleeDamageCheck damageCheck = colliderObj.GetComponent<MeleeDamageCheck>();
        if(damageCheck != null)
        {
            damageCheck.Init(distance, 1, damage, typeof(BossHitEffect), skillDuration);
        }

        // 콜라이더 위치 원래대로 돌리기
        yield return new WaitForSeconds(colliderDuration);
        GameObject.Destroy(colliderObj);
    }
}
