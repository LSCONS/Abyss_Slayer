using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class RestState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public override Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnExit 실행");
#endif
        UIManager.Instance.CloseUI(UISceneType.Rest);
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnRunnerEnter 실행");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;

        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.3f);

        await SoundManager.Instance.Init(ESceneName.RestScene);
        state?.SetLoadingBarValue(0.4f);

        var runner = RunnerManager.Instance.GetRunner();

        //TODO 서버에서 허수아비 소환하고 실제로 생성됐는지 await 걸어야함(보스랑 연결됐는지 확인)

#if MoveSceneDebug
        Debug.Log("Rpc 래디 해주세용");
#endif
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();
        state?.SetLoadingBarValue(0.7f);

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }
        //TODO: 플레이어 위치 동기화도 필요함


#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;


#if MoveSceneDebug
        Debug.Log("RestState 개방");
#endif
        UIManager.Instance.OpenUI(UISceneType.Rest);
        if (runner.IsServer)
        {
#if MoveSceneDebug
        Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif
            ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();

#if MoveSceneDebug
        Debug.Log("1초만 기다려줘");
#endif
            await Task.Delay(100);
            ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();

#if MoveSceneDebug
        Debug.Log("loadingState 삭제");
#endif
            await runner.UnloadScene(SceneName.LoadingScene);
        }
    }
}
