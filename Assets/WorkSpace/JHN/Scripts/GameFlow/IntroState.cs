using UnityEngine;
using System.Threading.Tasks;
public class IntroState : BaseGameState
{
    public override async Task OnEnter()
    {
        Debug.Log("IntroState OnEnter");
        LoadSceneManager.Instance.LoadScene(SceneName.IntroScene);

        // 모든 UI Addressables 로드
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        await ChangeState(new StartState());
    }

    public override async Task OnExit()
    {
        Debug.Log("IntroState OnExit");
        await Task.CompletedTask;   // 아무 일도 안함
    }
}
