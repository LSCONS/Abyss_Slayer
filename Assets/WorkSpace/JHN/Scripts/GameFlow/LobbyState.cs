using System.Threading.Tasks;
using UnityEngine;
public class LobbyState : BaseGameState
{   
    public override async Task OnEnter()
    {
        Debug.Log("LobbyState OnEnter");

        await LoadSceneManager.Instance.LoadScene(SceneName.LobbyScene);
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Lobby);
        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("LobbyState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Lobby);
        await Task.CompletedTask;
    }
}
