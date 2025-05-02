using System.Threading.Tasks;
using UnityEngine;
public class LobbyState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;

    public override async Task OnEnter()
    {
        Debug.Log("LobbyState OnEnter");

        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Lobby);

        await SoundManager.Instance.Init(EGameState.Lobby);
        SoundManager.Instance.PlayBGM(EGameState.Lobby, 1);

        await Task.CompletedTask;

        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.selectedCharacterClass);
        }
    }

    public override async Task OnExit()
    {
        Debug.Log("LobbyState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Lobby);
       // UIManager.Instance.CleanupUIMap();
        SoundManager.Instance.UnloadSoundsByState(EGameState.Lobby);



        await Task.CompletedTask;
    }
}
