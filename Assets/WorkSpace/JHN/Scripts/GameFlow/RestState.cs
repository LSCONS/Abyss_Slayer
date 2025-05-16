using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class RestState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public override async Task OnEnter()
    {
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Rest);

        // 브금 init
        SoundManager.Instance.Init(ESceneName.Rest);

        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Rest);
        // UIManager.Instance.CleanupUIMap();
        // UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        Debug.Log("데이터 Init하자");
        UIManager.Instance.Init();
        await SoundManager.Instance.Init(ESceneName.Rest);
        var runner = RunnerManager.Instance.GetRunner();


        Debug.Log("프로그래스 바 끝났는지 확인하자");
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        if (state != null)
        {
            state.IsLoadFast = true;
            await state.TaskProgressBar;
        }

        Debug.Log($"{ServerManager.Instance.ThisPlayerRef}: 준비 완료");
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();


        Debug.Log("서버야 모든 데이터가 유효하니?");
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }
        //TODO: 플레이어 위치 동기화도 필요함



        Debug.Log("RestState 개방");
        UIManager.Instance.OpenUI(UISceneType.Rest);
        if (runner.IsServer)
        {
            Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
            ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();
            Debug.Log("1초만 기다려줘");
            await Task.Delay(1000);
            ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
            Debug.Log("loadingState 삭제");
            await runner.UnloadScene(SceneName.LoadingScene);
        }
    }
}
