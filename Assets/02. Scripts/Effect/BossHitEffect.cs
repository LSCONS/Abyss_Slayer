using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitEffect : BasePoolable
{
    [Serializable] 
    public class HitEffectEntry
    {
        public EHitEffectType type;
        public AnimationClip clip;
    }

    [SerializeField] private Animator animator;
    [field: Header("사용할 모든 애니메이션 클립 넣기")]
    [SerializeField] private List<HitEffectEntry> hitEffects = new();       // 이펙트 애니메이션 클립 넣기 (이거 이넘값으로 사용할거임)

    private Dictionary<EHitEffectType, AnimationClip> effectMap = new();

    // 다 딕셔너리에 넣어주기
    private void Awake()
    {
        foreach (var entry in hitEffects)
        {
            if (!effectMap.ContainsKey(entry.type))
                effectMap.Add(entry.type, entry.clip);
            Debug.Log($"등록된 이펙트: {entry.type} -> {entry.clip?.name}");

        }
    }

    /// <summary>
    /// 이펙트 재생
    /// </summary>
    /// <param name="type">이펙트 타입</param>
    public void PlayEffect(EHitEffectType type)
    {
        if(effectMap.TryGetValue(type, out AnimationClip clip) && animator != null)
        {
            Debug.Log($"[BossHitEffect] {type} 재생 → 클립 이름: {clip.name}");
            animator.Play(clip.name);
        }
        else
        {
            Debug.LogWarning($"[BossHitEffect] {type}에 해당하는 클립이 없습니다.");
        }
    }

    public override void Init()
    {
        gameObject.SetActive(true);
    }

    public override void AutoReturn(float aliveTime)
    {
        StartCoroutine(EnableForAliveTime(aliveTime));
    }

    /// <summary>
    /// 풀 객체인지 확인해서 풀 객체면 풀에 반환, 풀 객체가 아니면 그냥 파괴 (aliveTime 후에)
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableForAliveTime(float aliveTime)
    {
        yield return new WaitForSeconds(aliveTime);

        // 풀 객체인지 확인해서 풀 객체면 풀에 반환, 풀 객체가 아니면 그냥 파괴 (aliveTime 후에)
        var poolable = GetComponent<BasePoolable>();
        if (poolable != null)
        {
            poolable.ReturnToPool();
        }
        else    // 풀 객체가 아니면 그냥 파괴
        {
            Destroy(gameObject);
        }
    }
}

public enum EHitEffectType
{
    Normal, // 일반공격
    Dash,   // 마검사 대쉬 공격
}
