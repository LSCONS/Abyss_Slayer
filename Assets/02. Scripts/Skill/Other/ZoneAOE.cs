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

    private Player player;
    private Skill skill;
    public override void Init()
    {

    }
    public override void Init(Vector3 spawnPos, Vector2 size, float tickRate, float _duration, float damage, LayerMask targetLayer, string effectName)
    {
        // 기본 세팅
        this.duration = _duration;
        this.tickInterval = tickRate;
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.range = Mathf.Max(size.x, size.y);

        // 위치 세팅
        transform.position = spawnPos;

        // 애니메이터 세팅
        SetActiveAnimator(effectName);

        // 박스 콜라이더 세팅
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = size;

        // meleedamagecheck 세팅
        var meleeCheck = GetComponent<MeleeDamageCheck>();
        System.Type fxType = null;
        meleeCheck.Init(player, skill, size, new Vector2(0,0), damage, fxType, _duration);
        meleeCheck.SetRepeatMode(true, tickRate);

        gameObject.SetActive(true);

        // duration 후 풀에 자동 반환
        if (ReturnToPoolCoroutine != null) StopCoroutine(ReturnToPoolCoroutine);
        ReturnToPoolCoroutine = StartCoroutine(ReturnTOPool(_duration, targetLayer));
    }

    public void Init(Player player, Vector3 spawnPos, Vector2 size, float tickRate, float _duration, float damage, LayerMask targetLayer, string effectName)
    {
        this.player = player;
        Init(spawnPos, size, tickRate, _duration, damage, targetLayer, effectName);
    }

    public void Init(Player player, Skill skill, Vector3 spawnPos, Vector2 size, float tickRate, float _duration, float damage, LayerMask targetLayer, string effectName)
    {
        this.player = player;
        this.skill = skill;
        Init(player, spawnPos, size, tickRate, _duration, damage, targetLayer, effectName);
    }

    private IEnumerator ReturnTOPool(float seconds, LayerMask targetLayer)
    {
        yield return new WaitForSeconds(seconds);
        if (effectPrefab != null)
            effectPrefab.SetActive(false);
        ReturnToPool();
    }

    // 애니메이터 활성화
    private void SetActiveAnimator(string effectName){
        // 존 이펙트 활성화
        if (effectPrefab != null)
        {   // 이펙트 길이 맞추기
            effectPrefab.SetActive(true);
            var effectSprite = GetComponentInChildren<SpriteRenderer>();
            effectSprite.transform.localScale = Vector3.one;
            effectSprite.transform.localRotation = Quaternion.Euler(0, 0, 0);
            effectSprite.flipX = player.IsFlipX;

            var animator = effectPrefab.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = ChangeAnimatior(effectName);
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                // 첫 번째 클립 길이 가져오기
                var clips = animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    float clipLength = clips[0].length;
                    // clipLength 초짜리 애니메이션을 duration 초에 맞춰 재생
                    animator.speed = clipLength / duration;
                    // 애니메이션 처음부터 재생
                    animator.Play(clips[0].name, 0, 0f);
                }
            }
        }
    }

    // 애니메이터 가져오기 << 나중에 미리 캐싱해두기
    private RuntimeAnimatorController ChangeAnimatior(string effectName)
    {
       return Resources.Load<RuntimeAnimatorController>("Effect/Animator/" + effectName);   
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
