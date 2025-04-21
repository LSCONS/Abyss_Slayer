using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 범위 공격에 쓰일 장판 스크립트
public class ZoneAOE : BasePoolable
{

    private float duration;
    private float tickInterval;
    private float damage;
    private float range;
    private LayerMask targetLayer;

    private Coroutine tickCoroutine; // 틱뎀 줄 코루틴
    private Coroutine ReturnToPoolCoroutine; // 풀로 돌려보내는 코루틴
    private HashSet<IHasHealth> targetsInRange = new();

    [SerializeField] private GameObject effectPrefab; // 이펙트 프리팹

    public override void Init()
    {

    }
    public override void Init(Vector3 spawnPos, float sizeX, float sizeY, float tickRate, float _duration, float damage, LayerMask targetLayer)
    {
        // 기본 세팅
        this.duration = _duration;
        this.tickInterval = tickRate;
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.range = Mathf.Max(sizeX, sizeY);

        // 위치 세팅
        transform.position = spawnPos;

        // 존 이펙트 활성화
        if (effectPrefab != null)
        {   // 이펙트 길이 맞추기
            effectPrefab.SetActive(true);
            var animator = effectPrefab.GetComponentInChildren<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                // 첫 번째 클립 길이 가져오기
                var clips = animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    float clipLength = clips[0].length;
                    // clipLength 초짜리 애니메이션을 duration 초에 맞춰 재생
                    animator.speed = clipLength / _duration;
                    // 애니메이션 처음부터 재생
                    animator.Play(clips[0].name, 0, 0f);
                }
            }
        }

        // 박스 콜라이더 세팅
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(sizeX, sizeY);

        // meleedamagecheck 세팅
        var meleeCheck = GetComponent<MeleeDamageCheck>();
        System.Type fxType = null;
        meleeCheck.Init(sizeX, sizeY, damage, fxType, _duration);
        meleeCheck.SetRepeatMode(true, tickRate);

        gameObject.SetActive(true);

        // duration 후 풀에 자동 반환
        if(ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
        ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(_duration, targetLayer));
    }

    private IEnumerator ReturnTOPool(float seconds, LayerMask targetLayer)
    {
        yield return new WaitForSeconds(seconds);
        if (effectPrefab != null)
            effectPrefab.SetActive(false);
        ReturnToPool();
    }

    // 기즈모 범위 씬 뷰에서 범위 확인용
    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(col.size.x, col.size.y, 0f);
        Gizmos.DrawWireCube(transform.position, size);
    }

}
