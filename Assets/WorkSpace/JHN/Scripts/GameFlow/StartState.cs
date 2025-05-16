using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class StartState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("StartState OnEnter진입");
#endif
        UIManager.Instance.Init();
        await SoundManager.Instance.Init(ESceneName.StartScene);

        await Task.Delay(1000);

        SoundManager.Instance.PlayBGM(ESceneName.StartScene, 1);

#if MoveSceneDebug
        Debug.Log("서버에 연결 중");
#endif
        var temp = ServerManager.Instance.ConnectRoomSearch();
        await temp;


#if MoveSceneDebug
        Debug.Log("프로그래스 바 종료 시키기는 중");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        if (state != null)
        {
            state.IsLoadFast = true;
            await state.TaskProgressBar;
        }

#if MoveSceneDebug
        Debug.Log("StartState UI 오픈");
#endif
        UIManager.Instance.OpenUI(UISceneType.Start);
#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync(SceneName.LoadingScene);

        return;
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("StartScene OnExit 실행");
#endif
        SoundManager.Instance.UnloadSoundsByState(ESceneName.StartScene);
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("StartState OnRunnerEnter 실행");
#endif
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Start);

        SoundManager.Instance.Init(ESceneName.StartScene);
        SoundManager.Instance.PlayBGM(ESceneName.StartScene, 1);

        await Task.CompletedTask;
    }
}
