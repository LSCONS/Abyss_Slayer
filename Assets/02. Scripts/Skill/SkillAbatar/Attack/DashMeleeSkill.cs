using System;
using System.Collections;
using UnityEngine;



[CreateAssetMenu(fileName = "NewDashMeleeSkill", menuName = "Skill/MeleeAttack/Dash")]
public class DashMeleeSkill : RemoteZoneRangeSkill
{
    [field: Header("대시 시간")]
    [field: SerializeField] public float DashTime { get; private set; } = 0.5f;
    [field: Header("대시 거리")]
    [field: SerializeField] public float DashDistance { get; private set; } = 5f;

    public GameObject dashEffectPrefab;

    public override void UseSkill()
    {
        // 대시 방향 설정
        Vector2 dashDirection = player.IsFlipX ? Vector2.left : Vector2.right;

        // 대시 이후의 처리들 (데미지 넣을 콜라이더 이동 + )
        player.StartCoroutine(DashCoroutine(dashDirection, DashTime));
    }

    private IEnumerator DashCoroutine(Vector2 dashDirection, float dashDuration)
    {
        // 시작 위치 타겟위치 설정
        float time = 0;
        Vector2 startPos = player.transform.position;
        Vector2 targetPos = startPos + dashDirection * DashDistance + Vector2.up * 0.01f; ;

        // 대쉬 이펙트 생성
        GameObject dashEffect = null;
        if (dashEffectPrefab != null)
        {
            // 위치 설정
            Vector3 effectOffset = new Vector3(player.IsFlipX ? 1f : -1f, -0.5f, 0);   // 플립되어있으면 1f, 아니면 -1f
            Vector3 effectPos = player.transform.position + effectOffset;

            dashEffect = GameObject.Instantiate(dashEffectPrefab, effectPos, Quaternion.identity, player.transform);
            dashEffect.transform.right = dashDirection;

            // 이펙트 방향 설정
            SpriteRenderer effectSpriteRenderer = dashEffect.GetComponent<SpriteRenderer>();
            if (effectSpriteRenderer != null)
            {
                effectSpriteRenderer.flipX = dashDirection.x < 0;
            }
        }


        // dashDistance만큼을 dashDuration 시간동안 이동
        while (time < dashDuration)
        {
            player.playerRigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, time / dashDuration));
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 대시 이후 위치
        player.playerRigidbody.MovePosition(targetPos);

        // 이펙트 제거
        if (dashEffect != null)
        {
            GameObject.Destroy(dashEffect, 0.5f);
        }

        // 끝/중간 위치 설정
        Vector2 endPos = player.transform.position;
        Vector2 midPos = (startPos + endPos) / 2;
        float distance = Vector2.Distance(startPos, endPos);

        // 콜라이더 위치 저장 이동
        Vector2 originalPos = player.PlayerMeleeCollider.transform.position;
        // player.playerMeleeCollider.transform.position = midPos;

        ColliderSize = new Vector2(distance, 1.0f);
        ColliderOffset = Vector2.zero;

        PoolManager.Instance.Get<ZoneAOE>().Init(this, typeof(BossHitEffect));
    }
}
