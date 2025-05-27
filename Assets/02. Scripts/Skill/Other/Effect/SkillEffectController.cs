using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectController : BasePoolable
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Animator animator;

    public override void Spawned()
    {
        base.Spawned();
        if (fadeController == null)
            fadeController = GetComponentInChildren<FadeController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (fadeController == null) Debug.LogError("페이드 컨트롤러 빠짐");
        if (animator == null) Debug.LogError("animator 빠짐");
    }

    public override void Rpc_Init()
    {

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(int EClipNameInt, float skillEffectsDuration, Vector3 position, Vector3 scale)
    {
        // 시작할 때 페이드인
        gameObject.SetActive(true);
        transform.position = position;
        transform.localScale = scale;
        fadeController?.FadeIn(1f);     // 딱 보일 때까지 페이드인
        animator.Play(((ESkillStartClipName)EClipNameInt).ToString());
        AutoReturn(skillEffectsDuration);
    }

    /// <summary>
    /// 자동으로 풀로 리턴해주는 메서드
    /// </summary>
    /// <param name="aliveTime"></param>
    public override void AutoReturn(float aliveTime)
    {
        base.AutoReturn(aliveTime);
        StartCoroutine(FadeOutAndReturn(aliveTime));
    }

    /// <summary>
    /// 페이드 아웃 끝날 때 리턴해주는 메서드
    /// </summary>
    /// <param name="duration">지속시간</param>
    /// <returns></returns>
    private IEnumerator FadeOutAndReturn(float duration)
    {
        yield return new WaitForSeconds(duration - fadeDuration);

        fadeController?.FadeOut(() => Rpc_ReturnToPool()); // 페이드 아웃 끝나면 풀로 리턴 해줌
    }
}

public enum ESkillStartClipName
{
    None = 0,
    TankerSpellSkillEffect,
}
