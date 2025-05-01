using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LoadingState : BaseGameState
{
    public override UIType StateUIType => UIType.None;
    private EGameState nextStateEnum;
    private readonly UIType prevUIType;
    public LoadingState(EGameState nextState, UIType prevUIType)
    {
        this.nextStateEnum = nextState;
        this.prevUIType = prevUIType;
    }

    public override async Task OnEnter()
    {
        Debug.Log("LoadingState OnEnter");

        // 1. 로딩씬 로드 (Additive or Single 방식 중 선택)
        var loadingOp = SceneManager.LoadSceneAsync(SceneName.LoadingScene, LoadSceneMode.Additive);
        while (!loadingOp.isDone)
            await Task.Yield();

        // 프로그래스바 가져와
        ProgressBar progressBar = null;
        
        while((progressBar = GameObject.Find("ProgressBar")?.GetComponent<ProgressBar>()) == null)
        {
            await Task.Yield();
        }


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
        var sceneOp = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        sceneOp.allowSceneActivation = false;

        // 4. 프로그래스바 업데이트
        while (sceneOp.progress < 0.9f)
        {
            progressBar?.SetProgress(sceneOp.progress);
            await Task.Yield();
        }
        progressBar?.SetProgress(1f);

        // 5. UI 로드 여부 결정
        sceneOp.allowSceneActivation = true;
        while (!sceneOp.isDone) await Task.Yield();

        // 7. 최종 상태 진입
        await GameFlowManager.Instance.ChangeState(nextState);
    }

    private string GetSceneNameFromState(IGameState state)
    {
        return state switch
        {
            IntroState => SceneName.IntroScene,
            StartState => SceneName.StartScene,
            LobbyState => SceneName.LobbyScene,
            RestState => SceneName.RestScene,
            InGameState inGame => SceneName.BossScenePrefix + inGame.stageIndex, // 보스 인덱스 기반으로 생성
            _ => throw new System.Exception($"[LoadingState] Unknown state: {state.GetType().Name}")
        };
    }

    public override async Task OnExit()
    {
        Debug.Log("LoadingState OnExit");
        await Task.CompletedTask;
    }
    
}
