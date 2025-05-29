using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
using UnityEditor;

public enum ESceneName  // 게임 시작 상태
{
    LoadingScene = 0,
    IntroScene = 1,
    StartScene = 2,
    LobbyScene = 3,
    RestScene = 4,
    TutorialScene = 5,
    BattleScene = 6,
    EndingScene = 7,
}

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LoadingState prevLodingState { get; set; }
    public IGameState PrevState { get; set; }
    public IGameState CurrentState { get; set; }   // 지금 스테이트
    [SerializeField] private ESceneName startStateEnum = ESceneName.IntroScene;   // 시작 스테이트 인스펙터 창에서 설정가능하게 해줌
    IntroState IntroSceneState { get; set; } = new IntroState();
    StartState StartSceneState { get; set; } = new StartState();
    LobbyState LobbySceneState { get; set; } = new LobbyState();
    RestState RestSceneState { get; set; } = new RestState();
    BattleState BattleSceneState { get; set; } = new BattleState();
    EndingState EndingSceneState { get; set; } = new EndingState();
    TutorialState TutorialSceneState { get; set; } = new TutorialState();

    [HideInInspector] public CreditRoller endCredit;    // 엔딩 크래딧 캐싱

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        CurrentState = null;
        ClientSceneLoad(startStateEnum);    // 시작은 introstate
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeStateWithLoading(ESceneName nextEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeStateWithLoading");
#endif
        if (nextEnum == ESceneName.LoadingScene)
            return;

        PrevState = CurrentState;

        // 1) 이전 UIType 캐시
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
        Debug.Log("RpcServerSceneLoad");
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
        Debug.Log("ClientSceneLoad");
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
            int? funnelStep = GetFunnelStepForScene(baseGameState.SceneName, GameValueManager.Instance.CurrentStageIndex);
            if (funnelStep.HasValue)
                AnalyticsManager.SendFunnelStep(funnelStep.Value);
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
            int? funnelStep = GetFunnelStepForScene(baseGameState.SceneName, GameValueManager.Instance.CurrentStageIndex);
            if (funnelStep.HasValue)
                AnalyticsManager.SendFunnelStep(funnelStep.Value);
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
            ESceneName.IntroScene => IntroSceneState,
            ESceneName.StartScene => StartSceneState,
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
        if (scene == ESceneName.StartScene)
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

    private void Update()
    {
        if (CurrentState is BaseGameState baseState)
        {
            baseState.OnUpdate();
        }
    }


    public string GetSceneNameFromState(IGameState state)
    {
        return state switch
        {
            IntroState => "IntroScene",
            StartState => "StartScene",
            LobbyState => "LobbyScene",
            RestState => "RestScene",
            BattleState => "BossScene_" + GameValueManager.Instance.CurrentStageIndex.ToString(), // 보스 인덱스 기반으로 생성
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
