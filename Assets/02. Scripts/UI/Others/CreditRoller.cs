using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreditRoller : UIPopup
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollDuration = 10;
    private float speedMultiplier = 1f; // 현재 배속

    [SerializeField] private CanvasGroup thanksTextGroup;
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private GameObject showSpeed;

    // 스크롤 연출 끝나고 콜백
    public Task CreditEndTask =>creditEnd.Task;
    public TaskCompletionSource<bool> creditEnd = new();
    [SerializeField] private float thanksTextDuration = 5;

    public void StartScrollCredit()
    {
        scrollRect.gameObject.SetActive(true);
        StartCoroutine(ScrollCredits());
    }

    public void Start()
    {
        GameFlowManager.Instance.endCredit = this;
        scrollRect.gameObject.SetActive(false);
     
    }


    private void Update()
    {
        // 스페이스 키 누르면 2배속
        if (Input.GetKey(KeyCode.Space))
        {
            speedMultiplier = 2f;
            showSpeed.SetActive(true);
        }
        else
        {
            speedMultiplier = 1f;
            showSpeed.SetActive(false);
        }
    }

    private IEnumerator ScrollCredits()
    {
        UIManager.Instance.DelayRebuildLayout(this);
        // 중앙에서 시작
        var layout = scrollRect.content.GetComponent<VerticalLayoutGroup>();
        float halfHeight = scrollRect.viewport.rect.height / 2f;
        layout.padding.top = Mathf.RoundToInt(halfHeight);
        layout.padding.bottom = Mathf.RoundToInt(halfHeight * 2);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        // 젤 위에서 시작
        scrollRect.verticalNormalizedPosition = 1f;

        yield return new WaitForSeconds(1);

        float elapsed = 0f;
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime * speedMultiplier;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        // 크레딧 끝났을 때 감사 텍스트 출력
        yield return StartCoroutine(FadeInThanksText());

        // 감사 텍스트 끝나고 thanksTextDuration 만큼 기다렸다가 크레딧 완료 알림
        yield return new WaitForSeconds(thanksTextDuration);
        creditEnd.TrySetResult(true);
        try
        {
            ServerManager.Instance.ExitRoom();
        }
        catch
        {
            GameFlowManager.Instance.QuitGame();
        }
    }
    /// <summary>
    /// 감사 텍스트 페이드인
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeInThanksText()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            thanksTextGroup.alpha = alpha;
            yield return null;
        }
        thanksTextGroup.alpha = 1f;
    }

}
