using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;

public enum ESceneName  // 게임 시작 상태
{
    LoadingScene = 0,
    IntroScene = 1,
    StartScene = 2,
    LobbyScene = 3,
    RestScene = 4,
    Battle0Scene = 5,
    Battle1Scene = 6,
    Battle2Scene = 7,
    Battle3Scene = 8,
}

public class GameFlowManager : Singleton<GameFlowManager>
{
    public LoadingState prevLodingState { get; set; }
    public IGameState PrevState { get; set; }
    public IGameState CurrentState { get; set; }   // 지금 스테이트
    [SerializeField] private ESceneName startStateEnum = ESceneName.IntroScene;   // 시작 스테이트 인스펙터 창에서 설정가능하게 해줌
    public int CurrentStageIndex { get; private set; } = 0; // 보스 생성할 때 쓸 index

    IntroState  IntroSceneState     { get; set; } = new IntroState();
    StartState  StartSceneState     { get; set; } = new StartState();
    LobbyState  LobbySceneState     { get; set; } = new LobbyState();
    RestState RestSceneState { get; set; } = new RestState();
    BattleState BattleSceneState { get; set; } = new BattleState();

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


    public async void RpcServerSceneLoad(ESceneName nextStateEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("RpcServerSceneLoad");
#endif
        await ChangeRunnerStateWithLoading(nextStateEnum);
        return;
    }

    public async void ClientSceneLoad(ESceneName nextStateEnum)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ClientSceneLoad");
#endif
        await ChangeStateWithLoading(nextStateEnum);
        return;
    }


    //    // 2) 최초 진입: currentState == null
    //    if (currentState == null || currentState is LoadingState)
    //    {
    //        Debug.Log($"[GameFlowManager] 최초 진입 → LoadingState({nextEnum}) 경유");
    //        await ChangeState(new LoadingState(nextEnum));
    //        return;
    //    }

    //    // 3) 현재 상태도 BaseGameState여야 하고
    //    var currentBase = currentState as BaseGameState;
    //    if (currentBase == null)
    //    {
    //        Debug.LogError("[GameFlowManager] currentState가 BaseGameState가 아닙니다.");
    //        return;
    //    }

    //    // 4) UIType이 다른 경우: LoadingState 경유
    //    if (currentBase.StateUIType != nextBase.StateUIType)
    //    {
    //        Debug.Log($"[GameFlowManager] UIType 변경 → LoadingState({nextEnum}) 경유");
    //        await ChangeState(new LoadingState(nextEnum));
    //    }
    //    else
    //    {
    //        // 5) UIType이 같다면 바로 상태 전환
    //        Debug.Log($"[GameFlowManager] UIType 동일 → 바로 상태 전환: {nextEnum}");
    //        await ChangeState(nextBase);
    //    }
    //}

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
        await (CurrentState?.OnEnter() ?? Task.CompletedTask);
    }

    public async Task ChangeRunnerState(IGameState newState)
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("ChangeRunnerState");
#endif
        await (CurrentState?.OnExit() ?? Task.CompletedTask);
        CurrentState = newState;
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

    public void GoToRestState()
    {
#if AllMethodDebug || MoveSceneDebug
        Debug.Log("GoToRestState");
#endif
        RpcServerSceneLoad(ESceneName.RestScene);
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
            ESceneName.Battle0Scene => BattleSceneState,
            ESceneName.Battle1Scene => BattleSceneState,
            ESceneName.Battle2Scene => BattleSceneState,
            ESceneName.Battle3Scene => BattleSceneState,
            // EGameState.Loading => new LoadingState(state), // 애초에 로딩으로 씬 전환하면 그게 문제지

            _ => null
        };
    }

    private void Update()
    {
        if (CurrentState is BaseGameState baseState)
        {
            baseState.OnUpdate();
        }
    }
}
