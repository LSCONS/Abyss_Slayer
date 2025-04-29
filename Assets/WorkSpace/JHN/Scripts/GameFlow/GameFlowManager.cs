using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public enum GameStartState  // 게임 시작 상태
{
    Intro,
    Start,
    Lobby,
    Rest,
    Loading,
    Boss
}

public class GameFlowManager : Singleton<GameFlowManager>
{
    public IGameState currentState;
    private IGameState startState;

    [SerializeField] private GameStartState startStateEnum = GameStartState.Intro;

    private int currentStageIndex = 0; // 보스 생성할 때 쓸 index

    protected override void Awake() 
    {
        base.Awake();
        DontDestroyOnLoad(this);
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
        currentStageIndex++;
        await ChangeState(new InGameState(currentStageIndex));
    }

    public async Task GoToLobby()
    {
        await ChangeState(new LobbyState());
    }

    public async Task GoToRestState()
    {
        await ChangeState(new RestState(currentStageIndex));
    }


    private IGameState CreateStateFromEnum(GameStartState state)
    {
        return state switch
        {
            GameStartState.Intro => new IntroState(),
            GameStartState.Start => new StartState(),
            GameStartState.Lobby => new LobbyState(),
            GameStartState.Rest => new RestState(currentStageIndex),
            // GameStartState.Loading => new LoadingState(),
            GameStartState.Boss => new InGameState(currentStageIndex),
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
