using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class IntroState : BaseGameState
{
    public override UIType StateUIType => UIType.None;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("IntroState OnEnter 실행");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        state?.SetLoadingBarValue(0.3f);

        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.4f);

        await DataManager.Instance.Init();

#if MoveSceneDebug
        Debug.Log("프로그래스바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;

#if MoveSceneDebug
        Debug.Log("IntroState 오픈");
#endif
        UIManager.Instance.OpenUI(UISceneType.Intro);
#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync(SceneName.LoadingScene);
    }

    public override async Task OnExit()
    {
        Debug.Log("IntroState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Intro);
        UIManager.Instance.CleanupUIMap();

        await Task.CompletedTask;   // 아무 일도 안함
    }

    public override Task OnRunnerEnter()
    {
        return Task.CompletedTask;
    }

    public override void OnUpdate()
    {

    }
}
