using System.Threading.Tasks;
using UnityEngine;
public class LobbyState : BaseGameState
{   
    public override async Task OnEnter()
    {
        Debug.Log("LobbyState OnEnter");
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();
        await LoadSceneManager.Instance.LoadScene(SceneName.LobbyScene);
        await Task.CompletedTask;

    }

    public override async Task OnExit()
    {
        Debug.Log("LobbyState OnExit");
        await Task.CompletedTask;
    }
}
