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
        UIManager.Instance.CloseAllPermanent();
        UIManager.Instance.CloseAllPopup();

        UIManager.Instance.OpenUI(UISceneType.Lobby);

        await SoundManager.Instance.Init(EGameState.Lobby);
        SoundManager.Instance.PlayBGM(EGameState.Lobby, 1);

        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("LobbyState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Lobby);
        UIManager.Instance.CleanupUIMap();
        SoundManager.Instance.UnloadAllSounds();


        await Task.CompletedTask;
    }
}
