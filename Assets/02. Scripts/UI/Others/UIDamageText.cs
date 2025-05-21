using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIDamageText : BasePoolable
{
    private TextMeshProUGUI text;                          // 데미지 텍스트
    private CanvasGroup canvasGroup;                // 페이드 아웃에 사용

    private Vector3 worldPosition;                  // 기준 월드 좌표
    private float offsetY = 0f;                     // 위로 올라가는 애니메이션에 쓸 offset
    private bool follow = false;                    // 따라다닐지?
    public int OffsetIndex { get; set; }            // 몇 번째 줄인지 저장


    public override void Rpc_Init()
    {
    }

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 데미지 텍스트 보여줌
    /// </summary>
    /// <param name="damage">표시할 데미지 텍스트</param>
    /// <param name="worldPos">시작 기준 월드 좌표</param>
    public void Show(string damage, Vector3 worldPos)
    {
        this.worldPosition = worldPos;
        offsetY = 0f;
        follow = true;

        text.text = damage;
        canvasGroup.alpha = 1f;

        // 기존 트윈 제거
        DOTween.Kill(transform);
        DOTween.Kill(canvasGroup);

        // 애니메이션 y 축으로 위로 올라감
        DOTween.To(() => offsetY, y => offsetY = y, 1f, 1f).SetEase(Ease.OutCubic);

        // 알파값 조절해서 0으로 사라지게
        canvasGroup.DOFade(0f, 1f)
            .OnComplete(() =>
            {
                Rpc_ReturnToPool();
            });
    }

    private void Update()
    {
        if(follow)
        {
            // 월드 기준 위치로, 오프셋도 반영해줌
            Vector3 followPos = worldPosition + Vector3.up * offsetY;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(followPos);
            transform.position = screenPos;
        }
    }
}
