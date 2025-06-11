using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class IntroState : BaseGameState
{
    public override UIType StateUIType => UIType.None;

    public override ESceneName SceneName => ESceneName.IntroScene;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("IntroState OnEnter 실행");
#endif
        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;
        await ManagerHub.Instance.UIManager.LoadAllUI(UIType.NonGamePlay);
        ManagerHub.Instance.SoundManager.Init();
        state?.SetLoadingBarValue(0.3f);

        await ManagerHub.Instance.DataManager.Init();

        ManagerHub.Instance.UIManager.CreateAllUI(UIType.NonGamePlay);
        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.4f);

#if MoveSceneDebug
        Debug.Log("프로그래스바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1);
        await (state?.TaskProgressBar ?? Task.CompletedTask);

#if MoveSceneDebug
        Debug.Log("IntroState 오픈");
#endif
        ManagerHub.Instance.UIManager.OpenUI(UISceneType.Intro);
#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("IntroState OnExit");
#endif
        ManagerHub.Instance.UIManager.CloseUI(UISceneType.Intro);
        ManagerHub.Instance.UIManager.CleanupUIMap();

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
