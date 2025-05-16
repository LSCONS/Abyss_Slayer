using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Fusion;

public class LoadingState : BaseGameState
{
    public override UIType StateUIType => UIType.None;
    private ESceneName nextStateEnum;
    private readonly UIType prevUIType;
    private NetworkSceneAsyncOp NetworkSceneAsyncOp { get; set; }
    public bool IsLoadFast { get; set; } = false;
    public Task TaskProgressBar { get; set; } = null;
    public LoadingState(ESceneName nextState, UIType prevUIType)
    {
        this.nextStateEnum = nextState;
        this.prevUIType = prevUIType;
    }

    public override async Task OnEnter()
    {
        IsLoadFast = false;
        TaskProgressBar = null;
        if (GameFlowManager.Instance.PrevState != null)
        {
            // 1. 로딩씬 로드 (Additive or Single 방식 중 선택)
            var loadingOp = SceneManager.LoadSceneAsync(SceneName.LoadingScene, LoadSceneMode.Additive);
            while (!loadingOp.isDone)
                await Task.Yield();

            SceneManager.UnloadSceneAsync(GetSceneNameFromState(GameFlowManager.Instance.PrevState));
        }

        // 프로그래스바 가져와
        ProgressBar progressBar = null;

        
        while((progressBar = GameObject.Find("ProgressBar")?.GetComponent<ProgressBar>()) == null)
        {
            await Task.Yield();
        }

        progressBar.SetProgressValue(0);
        TaskProgressBar = SetProgressBar(progressBar);



        // 2. 다음 상태와 UIType 결정
        var nextState = GameFlowManager.Instance.CreateStateForPublic(nextStateEnum) as BaseGameState;
        if (nextState == null)
            throw new System.Exception($"Unknown state: {nextStateEnum}");
        UIType nextUIType = nextState.StateUIType;



        // 이제 유아이 타입 바뀌면 옛날 유아이타입은 다 삭제해야됨
        if (prevUIType !=UIType.None && prevUIType != nextUIType)
        {
            UIManager.Instance.ClearUI(prevUIType);
        }

        // 3. 다음 ui를 미리 로드 생성
        bool needLoadUI = (prevUIType == UIType.None) || (prevUIType != nextUIType);



        if (needLoadUI)
        {
            // 첫 진입이거나 UIType이 바뀔 때만 로드/생성
            await UIManager.Instance.LoadAllUI(nextUIType);
            UIManager.Instance.CreateAllUI(nextUIType);
        }

        // 4. 씬 로드
        string nextSceneName = GetSceneNameFromState(nextState);
        var sceneOp = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        while (!sceneOp.isDone) await Task.Yield();

        //다음 state로 이동
        await GameFlowManager.Instance.ChangeState(nextState);
    }


    /// <summary>
    /// 네트워크를 통해 씬을 변경할 때 사용할 메서드
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public override async Task OnRunnerEnter()
    {
        IsLoadFast = false;
        TaskProgressBar = null;

        // 1. 로딩씬 로드 (Additive or Single 방식 중 선택)
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            NetworkSceneAsyncOp = runner.LoadScene(SceneName.LoadingScene, LoadSceneMode.Additive);
            while (!NetworkSceneAsyncOp.IsDone)
                await Task.Yield();

            var temp = runner.UnloadScene(GetSceneNameFromState(GameFlowManager.Instance.PrevState));
            await temp;
        }

#if MoveSceneDebug
        Debug.Log("프로그래스 바 가져와서 설정하자");
#endif
        // 2. 프로그래스바 가져와서 설정
        ProgressBar progressBar = null;
        while ((progressBar = GameObject.Find("ProgressBar")?.GetComponent<ProgressBar>()) == null)
        {
            await Task.Yield();
        }

        progressBar.SetProgressValue(0);
        TaskProgressBar = SetProgressBar(progressBar);

#if MoveSceneDebug
        Debug.Log("UIType 결정하자");
#endif
        // 3.. 다음 상태와 UIType 결정
        var nextState = GameFlowManager.Instance.CreateStateForPublic(nextStateEnum) as BaseGameState;
        if (nextState == null)
            throw new System.Exception($"Unknown state: {nextStateEnum}");
        UIType nextUIType = nextState.StateUIType;

#if MoveSceneDebug
        Debug.Log("유아이타입 삭제하자");
#endif
        // 이제 유아이 타입 바뀌면 옛날 유아이타입은 다 삭제해야됨
        if (prevUIType != UIType.None && prevUIType != nextUIType)
        {
            UIManager.Instance.ClearUI(prevUIType);
        }



#if MoveSceneDebug
        Debug.Log("ui가 없다면 로드하자");
#endif
        // 3. 다음 ui를 미리 로드 생성
        bool needLoadUI = (prevUIType == UIType.None) || (prevUIType != nextUIType);

        if (needLoadUI)
        {
            // 첫 진입이거나 UIType이 바뀔 때만 로드/생성
            await UIManager.Instance.LoadAllUI(nextUIType);
            UIManager.Instance.CreateAllUI(nextUIType);
        }

#if MoveSceneDebug
        Debug.Log("씬을 불러오자");
#endif
        // 4. 씬 로드
        if (runner.IsServer)
        {
            var temp = runner.LoadScene(GetSceneNameFromState(nextState), LoadSceneMode.Additive);
            await temp;
        }

#if MoveSceneDebug
        Debug.Log("씬을 바꿔보자");
#endif
        // 7. 최종 상태 진입
        await GameFlowManager.Instance.ChangeRunnerState(nextState);
    }

    private async Task SetProgressBar(ProgressBar progressBar)
    {
        while (!(Mathf.Approximately(progressBar.progressBar.value, 1)))
        {
            int delayTime = Random.Range(10, 15);
            float varValue = Random.Range(0.003f, 0.005f);
            if (ServerManager.Instance.CheckAllPlayerIsReadyInClient() && IsLoadFast) 
            {
                varValue *= 5;
            }

            await Task.Delay(delayTime);
            progressBar.AddProgressValue(varValue);
        };
#if MoveSceneDebug
        Debug.Log("프로그래스바 종료");
#endif
        return;
    }

    private string GetSceneNameFromState(IGameState state)
    {
        return state switch
        {
            IntroState => SceneName.IntroScene,
            StartState => SceneName.StartScene,
            LobbyState => SceneName.LobbyScene,
            RestState => SceneName.RestScene,
            BattleState inGame => SceneName.BossScenePrefix + inGame.stageIndex, // 보스 인덱스 기반으로 생성
            _ => throw new System.Exception($"[LoadingState] Unknown state: {state.GetType().Name}")
        };
    }

    public override async Task OnExit()
    {
        Debug.Log("LoadingState OnExit");
        await Task.CompletedTask;
    }
}
