using UnityEngine;
using DG.Tweening;
using System;
using Fusion;
using System.Collections;
using UniRx;

// 두트윈으로 스프라이트 페이드 컨트롤을 할 수 있는 스크립트
[RequireComponent(typeof(NetworkObject), typeof(SpriteRenderer))]  // 스프라이트 랜더러 필수
public class FadeController : NetworkBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private Player player = null;

    private float originAlpha;

    public override void Spawned()
    {
        base.Spawned();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        originAlpha = spriteRenderer.color.a;
        gameObject.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(PlayerRef playerRef, float radius, float duration, float effectAmount)
    {
        player = ManagerHub.Instance.ServerManager.DictRefToPlayer[playerRef];
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        player.ArmorAmount -= effectAmount;
        transform.localScale = Vector3.one * radius * 2f;
        circleCollider.radius = radius;
        gameObject.SetActive(true);
        FadeIn(originAlpha);

        // 일정 시간 후 꺼주기
        Observable.Timer(System.TimeSpan.FromSeconds(duration))
        .Subscribe(_ =>
        {
            FadeOut(() =>
            {
                player.ArmorAmount += effectAmount;
                gameObject.SetActive(false);  // 페이드 컨트롤러 있으면 페이드 아웃 이후에 액티브 폴스
            });
        });
    }


    /// <summary>
    /// 목표 알파값까지 페이드 인 효과 내기
    /// </summary>
    /// <param name="targetAlpha">목표 알파값</param>
    private void FadeIn(float targetAlpha)
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
