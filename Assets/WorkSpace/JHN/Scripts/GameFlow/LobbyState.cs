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

        await SoundManager.Instance.Init(ESceneName.Lobby);
        SoundManager.Instance.PlayBGM(ESceneName.Lobby, 1);

        await Task.CompletedTask;

        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.CharacterClass);
        }
    }

    public override async Task OnExit()
    {
        Debug.Log("LobbyState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Lobby);
       // UIManager.Instance.CleanupUIMap();
        SoundManager.Instance.UnloadSoundsByState(ESceneName.Lobby);



        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        Debug.Log("LobbyState OnEnter");

        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Lobby);

        await SoundManager.Instance.Init(ESceneName.Lobby);
        SoundManager.Instance.PlayBGM(ESceneName.Lobby, 1);

        await Task.CompletedTask;

        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.CharacterClass);
        }
    }
}
