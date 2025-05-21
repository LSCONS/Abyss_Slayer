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
    public override ESceneName SceneName => ESceneName.LoadingScene;
    private ESceneName nextStateEnum;
    private readonly UIType prevUIType;
    private NetworkSceneAsyncOp NetworkSceneAsyncOp { get; set; }
    public float LoadingTargetValue { get; set; } = 0;
    public Task TaskProgressBar { get; set; } = null;
    public LoadingState(ESceneName nextState, UIType prevUIType)
    {
        this.nextStateEnum = nextState;
        this.prevUIType = prevUIType;
    }

    public void SetLoadingBarValue(float value)
    {
        LoadingTargetValue = value;
    }

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("LoadingState OnEnter");
#endif
        UIManager.Instance.popupBG?.SetActive(false);
        UIManager.Instance.CloseAllUIPopup();
        IGameState prev = GameFlowManager.Instance.PrevState;
        TaskProgressBar = null;
        LoadingTargetValue = 0;
        if (prev != null)
        {
            // 1. 로딩씬 로드 (Additive or Single 방식 중 선택)
            var loadingOp = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
            while (!loadingOp.isDone)
                await Task.Yield();


            if (!(prev is LobbyState || prev is BattleState || prev is RestState))
            {
                SceneManager.UnloadSceneAsync(GameFlowManager.Instance.GetSceneNameFromState(prev));
            }
        }

#if MoveSceneDebug
        Debug.Log("프로그래스바 가져와");
#endif
        // 프로그래스바 가져와
        ProgressBar progressBar = null;
        while ((progressBar = GameObject.Find("ProgressBar")?.GetComponent<ProgressBar>()) == null)
        {
            await Task.Yield();
        }

        progressBar.SetProgressValue(0);
        TaskProgressBar = SetProgressBar(progressBar);


#if MoveSceneDebug
        Debug.Log("다음 State 정보 가져와");
#endif
        // 2. 다음 상태와 UIType 결정
        var nextState = GameFlowManager.Instance.CreateStateForPublic(nextStateEnum) as BaseGameState;
        if (nextState == null)
            throw new System.Exception($"Unknown state: {nextStateEnum}");
        UIType nextUIType = nextState.StateUIType;


#if MoveSceneDebug
        Debug.Log("UI삭제 해");
#endif
        // 이제 유아이 타입 바뀌면 옛날 유아이타입은 다 삭제해야됨
        if (prevUIType != UIType.None && prevUIType != nextUIType)
        {
            UIManager.Instance.ClearUI(prevUIType);
        }

        // 3. 다음 ui를 미리 로드 생성
        bool needLoadUI = !(nextUIType == UIType.None) || (prevUIType == nextUIType);

        
#if MoveSceneDebug
        Debug.Log("UI 만들어");
#endif
        if (needLoadUI)
        {
            // 첫 진입이거나 UIType이 바뀔 때만 로드/생성
            await UIManager.Instance.LoadAllUI(nextUIType);
            UIManager.Instance.CreateAllUI(nextUIType);
        }


#if MoveSceneDebug
        Debug.Log("다음 씬 로드해");
#endif
        SetLoadingBarValue(0.1f);

        // 4. 씬 로드
        string nextSceneName = GameFlowManager.Instance.GetSceneNameFromState(nextState);
        var sceneOp = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        while (!sceneOp.isDone) await Task.Yield();


#if MoveSceneDebug
        Debug.Log("다음 씬으로 교체 해");
#endif
        SetLoadingBarValue(0.2f);

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
        UIManager.Instance.popupBG?.SetActive(false);
        UIManager.Instance.CloseAllUIPopup();
        TaskProgressBar = null;
        LoadingTargetValue = 0;

        // 1. 로딩씬 로드 (Additive or Single 방식 중 선택)
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            ServerManager.Instance.AllPlayerIsReadyFalse();
            NetworkSceneAsyncOp = runner.LoadScene("LoadingScene", LoadSceneMode.Additive);
            while (!NetworkSceneAsyncOp.IsDone)
                await Task.Yield();

            var temp = runner.UnloadScene(GameFlowManager.Instance.GetSceneNameFromState(GameFlowManager.Instance.PrevState));
            await temp;
            if (GameFlowManager.Instance.PrevState is BattleState) GameValueManager.Instance.NextStageIndex();
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

        SetLoadingBarValue(0.1f);

#if MoveSceneDebug
        Debug.Log("씬을 불러오자");
#endif
        // 4. 씬 로드
        if (runner.IsServer)
        {
            var temp = runner.LoadScene(GameFlowManager.Instance.GetSceneNameFromState(nextState), LoadSceneMode.Additive);
            await temp;
        }

        SetLoadingBarValue(0.2f);

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
            int delayTime = Random.Range(15, 20);
            float barValue = Random.Range(0.0005f, 0.003f);
            if (LoadingTargetValue > progressBar.progressBar.value)
            {
                barValue = barValue + barValue * (LoadingTargetValue - progressBar.progressBar.value) * 15;
            }
            await Task.Delay(delayTime);
            progressBar.AddProgressValue(barValue);
        }
        ;
#if MoveSceneDebug
        Debug.Log("프로그래스바 종료");
#endif
        return;
    }

    public override async Task OnExit()
    {
        Debug.Log("LoadingState OnExit");
        await Task.CompletedTask;
    }
}
