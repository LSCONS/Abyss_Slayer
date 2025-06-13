using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum ESceneName  // 게임 시작 상태
{
    StartScene = 0,
    LoadingScene = 1,
    IntroScene = 2,
    InputNameScene = 3,
    LobbyScene = 4,
    RestScene = 5,
    TutorialScene = 6,
    BattleScene = 7,
    EndingScene = 8,
}

public class GameFlowManager
{
    StartState StartSceneState { get; set; } = new();
    IntroState IntroSceneState { get; set; } = new();
    InputNameState InputNameSceneState { get; set; } = new();
    LobbyState LobbySceneState { get; set; } = new();
    RestState RestSceneState { get; set; } = new();
    BattleState BattleSceneState { get; set; } = new();
    EndingState EndingSceneState { get; set; } = new();
    TutorialState TutorialSceneState { get; set; } = new();

    public LoadingState prevLodingState { get; set; }
    public IGameState PrevState { get; set; }
    public IGameState CurrentState { get; set; }   // 현재 스테이트
    public CreditRoller endCredit { get; set; }    // 엔딩 크래딧 캐싱


    public void Update()
    {
        if (CurrentState is BaseGameState baseState)
        {
            baseState.OnUpdate();
        }
    }


    public void Init()
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log($"Init: {ManagerHub.Instance.GameValueManager.StartScene}");
#endif
        CurrentState = StartSceneState;
        ClientSceneLoad(ManagerHub.Instance.GameValueManager.StartScene);
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeStateWithLoading(ESceneName nextEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeStateWithLoading");
#endif
        if (nextEnum == ESceneName.LoadingScene) return;

        // 1) 이전 UIType 캐시
        PrevState = CurrentState;
        UIType prevUIType = UIType.None;
        if (CurrentState is BaseGameState prevBase)
            prevUIType = prevBase.StateUIType;

        // 2) 무조건 LoadingState로 경유
        await ChangeState(prevLodingState = new LoadingState(nextEnum, prevUIType));
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeRunnerStateWithLoading(ESceneName nextEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeRunnerStateWithLoading");
#endif
        if (nextEnum == ESceneName.LoadingScene)
            return;

        PrevState = CurrentState;

        // 1) 이전 UIType 캐시
        UIType prevUIType = UIType.None;
        if (CurrentState is BaseGameState prevBase)
            prevUIType = prevBase.StateUIType;

        // 2) 무조건 LoadingState로 경유
        await ChangeRunnerState(prevLodingState = new LoadingState(nextEnum, prevUIType));
    }


    /// <summary>
    /// 서버 환경에서 씬을 전환할 때 사용할 메서드
    /// 레스트, 배틀 씬으로 전환할 때 사용.
    /// </summary>
    /// <param name="nextStateEnum"></param>
    public async void RpcServerSceneLoad(ESceneName nextStateEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log($"RpcServerSceneLoad: {nextStateEnum}");
#endif
        await ChangeRunnerStateWithLoading(nextStateEnum);
        return;
    }


    /// <summary>
    /// 클라이언트 환경에서 씬을 전환할 때 사용할 메서드
    /// 인트로, 스타트, 로비로 전환할 때 사용.
    /// </summary>
    /// <param name="nextStateEnum"></param>
    public async void ClientSceneLoad(ESceneName nextStateEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log($"ClientSceneLoad : {nextStateEnum}");
#endif
        await ChangeStateWithLoading(nextStateEnum);
        return;
    }


    /// <summary>
    /// 상태 변경
    /// </summary>
    /// <param name="newState">새로운 상태</param>
    /// <returns>상태 변경 작업의 결과</returns>
    public async Task ChangeState(IGameState newState)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeState");
#endif
        await (CurrentState?.OnExit() ?? Task.CompletedTask);
        CurrentState = newState;
        // 퍼널 스텝 기록
        if (newState is BaseGameState baseGameState)
        {
            int? funnelStep = GetFunnelStepForScene(baseGameState.SceneName, ManagerHub.Instance.GameValueManager.CurrentStageIndex);
            if (funnelStep.HasValue)
                ManagerHub.Instance.AnalyticsManager.SendFunnelStep(funnelStep.Value);
        }
        await (CurrentState?.OnEnter() ?? Task.CompletedTask);
    }


    public async Task ChangeRunnerState(IGameState newState)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeRunnerState");
#endif
        await (CurrentState?.OnExit() ?? Task.CompletedTask);
        CurrentState = newState;
        // 퍼널 스텝 기록
        if (newState is BaseGameState baseGameState)
        {
            int? funnelStep = GetFunnelStepForScene(baseGameState.SceneName, ManagerHub.Instance.GameValueManager.CurrentStageIndex);
            if (funnelStep.HasValue)
                ManagerHub.Instance.AnalyticsManager.SendFunnelStep(funnelStep.Value);
        }
        await (CurrentState?.OnRunnerEnter() ?? Task.CompletedTask);
    }


    // 상태 생성가능하게
    public IGameState CreateStateForPublic(ESceneName stateEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("CreateStateForPublic");
#endif
        return CreateStateFromEnum(stateEnum);
    }


    private IGameState CreateStateFromEnum(ESceneName state)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("CreateStateFromEnum");
#endif
        return state switch
        {
            ESceneName.StartScene => StartSceneState,
            ESceneName.IntroScene => IntroSceneState,
            ESceneName.InputNameScene => InputNameSceneState,
            ESceneName.LobbyScene => LobbySceneState,
            ESceneName.RestScene => RestSceneState,
            ESceneName.BattleScene => BattleSceneState,
            ESceneName.EndingScene => EndingSceneState,
            ESceneName.TutorialScene => TutorialSceneState,
            // EGameState.Loading => new LoadingState(state), // 애초에 로딩으로 씬 전환하면 그게 문제지

            _ => null
        };
    }

    private int? GetFunnelStepForScene(ESceneName scene, int stageIndex)
    {
        // 엔딩 씬
        if (scene == ESceneName.EndingScene) 
            return 16;

        // 배틀 씬 (스테이지별로 구분)
        if (scene == ESceneName.BattleScene)
        {
            if (stageIndex == 0)
                return 3;   // 1스테이지 입장
            else if (stageIndex == 1)
                return 10;  // 2스테이지 입장
        }

        // 스타트 씬
        if (scene == ESceneName.InputNameScene)
            return 1;

        // 레스트 씬
        if (scene == ESceneName.RestScene)
            return stageIndex switch
            {
                0 => 2,
                1 => 9,
                _ => null
            };

        return null;
    }


    public string GetSceneNameFromState(IGameState state)
    {
        return state switch
        {
            StartState => "StartScene",
            IntroState => "IntroScene",
            InputNameState => "InputNameScene",
            LobbyState => "LobbyScene",
            RestState => "RestScene",
            BattleState => "BossScene_" + ManagerHub.Instance.GameValueManager.CurrentStageIndex.ToString(), // 보스 인덱스 기반으로 생성
            EndingState => "EndingScene",
            TutorialState => "TutorialScene",
            _ => throw new System.Exception($"[LoadingState] Unknown state: {state.GetType().Name}")
        };
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // 에디터 플레이 모드를 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드 앱을 종료
        Application.Quit();
#endif
    }
}
