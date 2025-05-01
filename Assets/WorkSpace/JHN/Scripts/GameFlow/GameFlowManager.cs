using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public enum EGameState  // 게임 시작 상태
{
    Intro,
    Start,
    Lobby,
    Rest,
    Loading,
    Battle
}

public class GameFlowManager : Singleton<GameFlowManager>
{
    [HideInInspector] public IGameState currentState;   // 지금 스테이트
    [SerializeField] private EGameState startStateEnum = EGameState.Intro;   // 시작 스테이트 인스펙터 창에서 설정가능하게 해줌

    public int CurrentStageIndex { get; private set; } = 0; // 보스 생성할 때 쓸 index

    protected override void Awake() 
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        await ChangeState(startStateEnum);    // 시작은 introstate
    }


    // loadingstate로 가서 상태 변경하게 하기
    public async Task ChangeState(EGameState nextEnum)
    {
        if (nextEnum == EGameState.Loading)
            return;

        // 1) 이전 UIType 캐시
        UIType prevUIType = UIType.None;
        if (currentState is BaseGameState prevBase)
            prevUIType = prevBase.StateUIType;

        // 2) 무조건 LoadingState로 경유
        await ChangeState(new LoadingState(nextEnum, prevUIType));
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
        if(currentState != null)
        {
            await currentState.OnExit();
        }

        currentState = newState;

        if(currentState != null)
        {
            await currentState.OnEnter();
        }
    }

    // 상태 생성가능하게
    public IGameState CreateStateForPublic(EGameState stateEnum)
    {
        return CreateStateFromEnum(stateEnum);
    }

    public async Task GoToNextBoss()
    {
        CurrentStageIndex++;
        await ChangeState(EGameState.Battle);
    }

    public async Task GoToLobby()
    {
        await ChangeState(EGameState.Lobby);
    }

    public async Task GoToRestState()
    {
        await ChangeState(EGameState.Rest);
    }


    private IGameState CreateStateFromEnum(EGameState state)
    {
        return state switch
        {
            EGameState.Intro => new IntroState(),
            EGameState.Start => new StartState(),
            EGameState.Lobby => new LobbyState(),
            EGameState.Rest => new RestState(CurrentStageIndex),
            EGameState.Battle => new InGameState(CurrentStageIndex),
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
