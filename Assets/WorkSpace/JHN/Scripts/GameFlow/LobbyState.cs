using Fusion;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("LobbyState Onenter 진입");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;

        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.3f);

#if MoveSceneDebug
        Debug.Log("방 생성 및 들어가기");
#endif
        if (ServerManager.Instance.IsServer)
            await ServerManager.Instance.InitHost();
        else
            await ServerManager.Instance.InitClient();


#if MoveSceneDebug
        Debug.Log("이미지 Sprite 불러오기");
#endif
        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            imageChange.Init(PlayerManager.Instance.CharacterClass);
        }


#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1f);
        await (state?.TaskProgressBar ?? Task.CompletedTask);

#if MoveSceneDebug
        Debug.Log("LobbyState 오픈");
#endif
        SoundManager.Instance.PlayBGM(EAudioClip.BGM_LobbyScene);
        UIManager.Instance.OpenUI(UISceneType.Lobby);

#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync(SceneName.LoadingScene);

        return;
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("LobbtyState OnExit 실행");
#endif
        await Task.CompletedTask;
    }

    public override Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("LobbtyState OnRunnerEnter 실행");
#endif
        return Task.CompletedTask;
    }
}
