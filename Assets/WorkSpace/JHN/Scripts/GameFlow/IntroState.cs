using UnityEngine;
using System.Threading.Tasks;
public class IntroState : BaseGameState
{
    private bool isTransitioning = false;


    public override async Task OnEnter()
    {
        Debug.Log("IntroState OnEnter");
        await LoadSceneManager.Instance.LoadScene(SceneName.IntroScene);

        // 모든 UI Addressables 로드
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Intro);
    }

    public override async Task OnExit()
    {
        Debug.Log("IntroState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Intro);
        UIManager.Instance.CleanupUIMap();

        await Task.CompletedTask;   // 아무 일도 안함
    }

    public override async void OnUpdate()
    {
        if(isTransitioning){
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isTransitioning = true;
            await ChangeState(new StartState());
        }
    }
}
