using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class InputNameState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override ESceneName SceneName => ESceneName.StartScene;
    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("StartState OnEnter진입");
#endif
        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;
        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.3f);


#if MoveSceneDebug
        Debug.Log("서버에 연결 중");
#endif
        var temp = ManagerHub.Instance.ServerManager.ConnectRoomSearch();
        await temp;


#if MoveSceneDebug
        Debug.Log("프로그래스 바 종료 시키기는 중");
#endif
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;

#if MoveSceneDebug
        Debug.Log("StartState UI 오픈");
#endif
        ManagerHub.Instance.SoundManager.PlayBGM(EAudioClip.BGM_StartScene);
        ManagerHub.Instance.UIManager.OpenUI(UISceneType.Start);
        ManagerHub.Instance.UIConnectManager.UIStartTitle.TextUpdate();
#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync("LoadingScene");

        return;
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("StartScene OnExit 실행");
#endif
        await Task.CompletedTask;
    }

    public override Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("StartState OnRunnerEnter 실행");
#endif
        return Task.CompletedTask;
    }
}
