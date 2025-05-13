using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectController : BasePoolable
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if(fadeController==null)
            fadeController = GetComponentInChildren<FadeController>();
        if(animator==null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (fadeController == null) Debug.LogError("페이드 컨트롤러 빠짐");
        if (animator == null) Debug.LogError("animator 빠짐");


    }
    public override void Init()
    {
        // 시작할 때 페이드인
        gameObject.SetActive(true);
        fadeController?.FadeIn(1f);     // 딱 보일 때까지 페이드인
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

        fadeController?.FadeOut(()=> ReturnToPool()); // 페이드 아웃 끝나면 풀로 리턴 해줌
    }

    // clip play해주는 메서드
    public void PlayClip(string clipName)
    {
        if (!string.IsNullOrEmpty(clipName))
        {
            animator.Play(clipName);
        }
    }
}
