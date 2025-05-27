using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// Unity Analytics의 기본 매니저 클래스
/// Unity Services 초기화 및 공통 이벤트 전송 기능을 제공
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static AnalyticsManager instance;
    private static bool isInitialized = false;

    // 외부에서 접근 가능한 인스턴스 프로퍼티
    public static AnalyticsManager Instance => instance;

    /// <summary>
    /// 싱글톤 패턴 구현을 위한 Awake 메서드
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Unity Services 초기화 및 데이터 수집 시작
    /// </summary>
    async void Start()
    {
        try
        {            
            await UnityServices.InitializeAsync();
            
            AnalyticsService.Instance.StartDataCollection();
            isInitialized = true;
            Debug.LogAssertion("[Analytics] Analytics data collection started successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogAssertion($"[Analytics] Unity Services initialization failed: {e.Message}\n{e.StackTrace}");
            isInitialized = false;
        }
    }

    /// <summary>
    /// 애널리틱스 서비스 초기화 상태 확인
    /// </summary>
    public static bool IsInitialized => isInitialized;

    /// <summary>
    /// 애널리틱스 이벤트를 전송하는 공통 메서드
    /// </summary>
    public static void SendAnalyticsEvent(string eventName, Dictionary<string, object> parameters)
    {
        if (!isInitialized)
        {
            Debug.LogAssertion($"[Analytics] Service not initialized. Event '{eventName}' not sent.");
            return;
        }

        try
        {
            var analyticsEvent = new CustomEvent(eventName);
            
            foreach (var param in parameters)
            {
                analyticsEvent[param.Key] = param.Value;
            }
            
            AnalyticsService.Instance.RecordEvent(analyticsEvent);
            Debug.Log($"[Analytics] Event '{eventName}' sent successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogAssertion($"[Analytics] Failed to send event '{eventName}': {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 퍼널 단계 이벤트 전송 함수
    /// </summary>
    /// <param name="stepNumber">퍼널 단계 번호 (1~16)</param>
    public static void SendFunnelStep(int stepNumber)
    {
        if (!isInitialized) return;

        var funnelEvent = new CustomEvent("Funnel_Step");
        funnelEvent["Funnel_Step_Number"] = stepNumber;
        AnalyticsService.Instance.RecordEvent(funnelEvent);
        Debug.LogAssertion($"[Analytics] Funnel Step Sent: {stepNumber}");
    }
}
