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
    public IGameState currentState;
    private IGameState startState;

    [SerializeField] private EGameState startStateEnum = EGameState.Intro;

    public int CurrentStageIndex { get; private set; } // 보스 생성할 때 쓸 index

    protected override void Awake() 
    {
        base.Awake();
        DontDestroyOnLoad(this);
        CurrentStageIndex = 0;
    }

    private async void Start()
    {
        startState = CreateStateFromEnum(startStateEnum);
        await ChangeState(startState);    // 시작은 introstate
    }

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

    public async Task GoToNextBoss()
    {
        CurrentStageIndex++;
        await ChangeState(new InGameState(CurrentStageIndex));
    }

    public async Task GoToLobby()
    {
        await ChangeState(new LobbyState());
    }

    public async Task GoToRestState()
    {
        await ChangeState(new RestState(CurrentStageIndex));
    }


    private IGameState CreateStateFromEnum(EGameState state)
    {
        return state switch
        {
            EGameState.Intro => new IntroState(),
            EGameState.Start => new StartState(),
            EGameState.Lobby => new LobbyState(),
            EGameState.Rest => new RestState(CurrentStageIndex),
            // GameStartState.Loading => new LoadingState(),
            EGameState.Battle => new InGameState(CurrentStageIndex),
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
