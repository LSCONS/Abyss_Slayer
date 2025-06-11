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
public class AnalyticsManager
{
    // 애널리틱스 서비스 초기화 상태 확인용 플래그
    private static bool isInitialized = false;

    // 애널리틱스 서비스 초기화 상태 프로퍼티
    public static bool IsInitialized => isInitialized;
    /// <summary>
    /// Unity Services 초기화 및 데이터 수집 시작
    /// 초기화 실패 시 에러 로그를 출력하고 isInitialized 플래그를 false로 설정합니다.
    /// </summary>
    public async void Init()
    {
        try
        {            
            // Unity Services 비동기 초기화
            await UnityServices.InitializeAsync();
            
            // 애널리틱스 데이터 수집 시작
            AnalyticsService.Instance.StartDataCollection();
            // 초기화 성공 플래그 설정
            isInitialized = true;
            // 초기화 성공 로그 출력
#if AnalyticsDebug
            Debug.LogAssertion("[Analytics] Analytics data collection started successfully");
#endif
        }
        catch (System.Exception e)
        {
            // 초기화 실패 시 상세 에러 로그 출력
            Debug.LogAssertion($"[Analytics] Unity Services initialization failed: {e.Message}\n{e.StackTrace}");
            // 초기화 실패 플래그 설정
            isInitialized = false;
        }
    }

    /// <summary>
    /// 애널리틱스 이벤트를 전송하는 공통 메서드
    /// </summary>
    /// <param name="eventName">전송할 이벤트의 이름</param>
    /// <param name="parameters">이벤트와 함께 전송할 파라미터 딕셔너리</param>
    public void SendAnalyticsEvent(string eventName, Dictionary<string, object> parameters)
    {
        // 서비스 초기화 상태 확인
        if (!isInitialized)
        {
            // 초기화되지 않은 경우 경고 로그 출력 후 종료
            Debug.LogAssertion($"[Analytics] Service not initialized. Event '{eventName}' not sent.");
            return;
        }

        try
        {
            // 새로운 커스텀 이벤트 생성
            var analyticsEvent = new CustomEvent(eventName);
            
            // 파라미터 딕셔너리의 모든 항목을 이벤트에 추가
            foreach (var param in parameters)
            {
                analyticsEvent[param.Key] = param.Value;
            }
            
            // 이벤트 전송
            AnalyticsService.Instance.RecordEvent(analyticsEvent);
            // 전송 성공 로그 출력
#if AnalyticsDebug
            Debug.LogAssertion($"[Analytics] Event '{eventName}' sent successfully");
#endif
        }
        catch (System.Exception e)
        {
            // 이벤트 전송 실패 시 상세 에러 로그 출력
            Debug.LogAssertion($"[Analytics] Failed to send event '{eventName}': {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 퍼널 단계 이벤트를 전송하는 특수 메서드
    /// </summary>
    /// <param name="stepNumber">퍼널 단계 번호 (1~16)</param>
    /// <remarks>
    /// 유저의 퍼널 진행 단계를 추적하기 위한 특수 이벤트를 전송합니다.
    /// 서비스가 초기화되지 않은 경우 이벤트를 전송하지 않습니다.
    /// 이벤트 전송 시 'Funnel_Step' 이벤트 이름과 함께 단계 번호를 파라미터로 전송합니다.
    /// </remarks>
    public void SendFunnelStep(int stepNumber)
    {
        // 서비스 초기화 상태 확인
        if (!isInitialized) return;

        // 퍼널 단계 이벤트 생성
        var funnelEvent = new CustomEvent("Funnel_Step");
        // 단계 번호 파라미터 추가
        funnelEvent["Funnel_Step_Number"] = stepNumber;
        // 이벤트 전송
        AnalyticsService.Instance.RecordEvent(funnelEvent);
        // 전송 성공 로그 출력
#if AnalyticsDebug
        Debug.LogAssertion($"[Analytics] Funnel Step Sent: {stepNumber}");
#endif
    }
}
