using Fusion;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override ESceneName SceneName => ESceneName.LobbyScene;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("LobbyState Onenter 진입");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        GameValueManager.Instance.ResetStageIndex();
        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.3f);

#if MoveSceneDebug
        Debug.Log("방 생성 및 들어가기");
#endif
        if (ServerManager.Instance.IsServer)
            await ServerManager.Instance.InitHost();
        else
            await ServerManager.Instance.InitClient();

        ServerManager.Instance.LobbySelectPanel.JoinRoom();

#if MoveSceneDebug
        Debug.Log("이미지 Sprite 불러오기");
#endif
        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            await ServerManager.Instance.WaitForThisPlayerDataAsync();
            NetworkData data = ServerManager.Instance.ThisPlayerData;
            imageChange.Init(data.Class, data.HairStyleKey, data.SkinKey, data.FaceKey);
        }
        ServerManager.Instance.CustomPanelManager.ApplyPreview();

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1f);
        await state?.TaskProgressBar;

#if MoveSceneDebug
        Debug.Log("LobbyState 오픈");
#endif
        SoundManager.Instance.PlayBGM(EAudioClip.BGM_LobbyScene);
        UIManager.Instance.OpenUI(UISceneType.Lobby);

#if MoveSceneDebug
        Debug.Log("LoadingScene 삭제");
#endif
        SceneManager.UnloadSceneAsync("LoadingScene");

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
