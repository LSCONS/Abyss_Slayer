using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;

public enum ESceneName  // 게임 시작 상태
{
    Intro = 0,
    Start = 1,
    Lobby = 2,
    Loading = 3,
    Rest = 4,
    Battle0 = 5,
    Battle1 = 6,
    Battle2 = 7,
    Battle3 = 8,
}

public class GameFlowManager : Singleton<GameFlowManager>
{
    public IGameState currentState { get; set; }   // 지금 스테이트
    [SerializeField] private ESceneName startStateEnum = ESceneName.Intro;   // 시작 스테이트 인스펙터 창에서 설정가능하게 해줌
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
        ClientSceneLoad(startStateEnum);    // 시작은 introstate
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeState(ESceneName nextEnum)
    {
        if (nextEnum == ESceneName.Loading)
            return;

        // 1) 이전 UIType 캐시
        UIType prevUIType = UIType.None;
        if (currentState is BaseGameState prevBase)
            prevUIType = prevBase.StateUIType;

        // 2) 무조건 LoadingState로 경유
        await ChangeState(new LoadingState(nextEnum, prevUIType));
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeRunnerState(ESceneName nextEnum)
    {
        if (nextEnum == ESceneName.Loading)
            return;

        // 1) 이전 UIType 캐시
        UIType prevUIType = UIType.None;
        if (currentState is BaseGameState prevBase)
            prevUIType = prevBase.StateUIType;

        // 2) 무조건 LoadingState로 경유
        await ChangeRunnerState(new LoadingState(nextEnum, prevUIType));
    }


    public async void RpcServerSceneLoad(ESceneName nextStateEnum)
    {
        await ChangeRunnerState(nextStateEnum);
    }

    public async void ClientSceneLoad(ESceneName nextStateEnum)
    {
        await ChangeState(nextStateEnum);
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
        if (currentState != null)
        {
            await currentState.OnExit();
        }

        currentState = newState;

        if (currentState != null)
        {
            await currentState.OnEnter();
        }
    }

    public async Task ChangeRunnerState(IGameState newState)
    {
        if (currentState != null)
        {
            await currentState.OnExit();
        }

        currentState = newState;

        if (currentState != null)
        {
            await currentState.OnRunnerEnter();
        }
    }

    // 상태 생성가능하게
    public IGameState CreateStateForPublic(ESceneName stateEnum)
    {
        return CreateStateFromEnum(stateEnum);
    }

    public void GoToRestState()
    {
        RpcServerSceneLoad(ESceneName.Rest);
    }


    private IGameState CreateStateFromEnum(ESceneName state)
    {
        return state switch
        {
            ESceneName.Intro => IntroSceneState,
            ESceneName.Start => StartSceneState,
            ESceneName.Lobby => LobbySceneState,
            ESceneName.Rest => RestSceneState,
            ESceneName.Battle0 => BattleSceneState,
            ESceneName.Battle1 => BattleSceneState,
            ESceneName.Battle2 => BattleSceneState,
            ESceneName.Battle3 => BattleSceneState,
            // EGameState.Loading => new LoadingState(state), // 애초에 로딩으로 씬 전환하면 그게 문제지

            _ => null
        };
    }

    private void Update()
    {
        if (currentState is BaseGameState baseState)
        {
            baseState.OnUpdate();
        }
    }
}
