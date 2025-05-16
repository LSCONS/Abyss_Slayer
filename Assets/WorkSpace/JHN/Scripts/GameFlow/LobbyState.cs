using Fusion;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;

    public override async Task OnEnter()
    {
        UIManager.Instance.Init();

        await SoundManager.Instance.Init(ESceneName.Lobby);
        SoundManager.Instance.PlayBGM(ESceneName.Lobby, 1);

        if (ServerManager.Instance.IsServer)
            await ServerManager.Instance.InitHost();
        else
            await ServerManager.Instance.InitClient();

        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.CharacterClass);
        }

        LoadingState state = GameFlowManager.Instance.prevLodingState;
        if (state != null)
        {
            state.IsLoadFast = true;
            await state.TaskProgressBar;
        }

        UIManager.Instance.OpenUI(UISceneType.Lobby);

        SceneManager.UnloadSceneAsync(SceneName.LoadingScene);

        return;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Lobby);
       // UIManager.Instance.CleanupUIMap();
        SoundManager.Instance.UnloadSoundsByState(ESceneName.Lobby);

        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Lobby);

        SoundManager.Instance.Init(ESceneName.Lobby);
        SoundManager.Instance.PlayBGM(ESceneName.Lobby, 1);

        await Task.CompletedTask;

        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.CharacterClass);
        }
    }
}
