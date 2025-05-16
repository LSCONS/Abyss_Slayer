using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class IntroState : BaseGameState
{
    private bool isTransitioning = false;
    public override UIType StateUIType => UIType.None;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("IntroState OnEnter 실행");
#endif
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();

#if MoveSceneDebug
        Debug.Log("프로그래스바 끝날 때까지 대기");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        if (state != null)
        {
            state.IsLoadFast = true;
            await state.TaskProgressBar;
        }

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

    public override async Task OnRunnerEnter()
    {
        Debug.Log("IntroState OnEnter");
        await LoadSceneManager.Instance.LoadScene(SceneName.IntroScene);

        // 모든 UI Addressables 로드
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Intro);
    }

    public override async void OnUpdate()
    {
        if(isTransitioning){
            return;
        }
    }
}
