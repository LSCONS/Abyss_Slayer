using UnityEngine;
using DG.Tweening;
using System;

// 두트윈으로 스프라이트 페이드 컨트롤을 할 수 있는 스크립트
[RequireComponent(typeof(SpriteRenderer))]  // 스프라이트 랜더러 필수
public class FadeController : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 목표 알파값까지 페이드 인 효과 내기
    /// </summary>
    /// <param name="targetAlpha">목표 알파값</param>
    public void FadeIn(float targetAlpha)
    {
        SetAlpha(targetAlpha);
    }

    /// <summary>
    /// 목표 알파값까지 페이드 인 효과 내기
    /// </summary>
    /// <param name="targetAlpha">목표 알파값</param>
    /// <param name="onComplete">페이드 인 완료 후 실행할 콜백</param>
    public void FadeIn(float targetAlpha, Action onComplete = null)
    {
        SetAlpha(targetAlpha, onComplete);
    }

    /// <summary>
    /// 알파값 0으로 천천히 사라지는 효과 내기
    /// </summary>
    /// <param name="onComplete">페이드 아웃 완료 후 실행할 콜백</param>
    public void FadeOut(Action onComplete = null)
    {
        SetAlpha(0f, onComplete);
    }

    // 스프라이트의 알파값 조절
    private void SetAlpha(float targetAlpha, Action onComplete = null)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(targetAlpha, fadeDuration)
                          .OnComplete(()=> onComplete?.Invoke());
        }
    }
}
