using Fusion;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;
public class LobbyState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override ESceneName SceneName => ESceneName.LobbyScene;

    public override async Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("LobbyState Onenter 진입");
#endif
        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;
        ManagerHub.Instance.GameValueManager.ResetStageIndex();
        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.3f);

#if MoveSceneDebug
        Debug.Log("방 생성 및 들어가기");
#endif
        if (ManagerHub.Instance.ServerManager.IsServer)
            await ManagerHub.Instance.ServerManager.InitHost();
        else
            await ManagerHub.Instance.ServerManager.InitClient();

        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel.JoinRoom();

#if MoveSceneDebug
        Debug.Log("이미지 Sprite 불러오기");
#endif
        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            await ManagerHub.Instance.ServerManager.WaitForThisPlayerDataAsync();
            NetworkData data = ManagerHub.Instance.ServerManager.ThisPlayerData;
            imageChange.Init(data.Class, data.HairStyleKey, data.SkinKey, data.FaceKey);
        }
        ManagerHub.Instance.UIConnectManager.UICustomPanelManager.ApplyPreview();

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1f);
        await state?.TaskProgressBar;

#if MoveSceneDebug
        Debug.Log("LobbyState 오픈");
#endif
        ManagerHub.Instance.SoundManager.PlayBGM(EAudioClip.BGM_LobbyScene);
        ManagerHub.Instance.UIManager.OpenUI(UISceneType.Lobby);

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

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("LobbtyState OnRunnerEnter 실행");
#endif
        NetworkRunner runner = RunnerManager.Instance.GetRunner();

        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;
        ManagerHub.Instance.GameValueManager.ResetStageIndex();
        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.3f);

        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel.JoinRoom();


#if MoveSceneDebug
        Debug.Log("이미지 Sprite 불러오기");
#endif
        SpriteImageChange[] imageChanges = Util.FindObjectsByTypeDebug<SpriteImageChange>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (SpriteImageChange imageChange in imageChanges)
        {
            await ManagerHub.Instance.ServerManager.WaitForThisPlayerDataAsync();
            NetworkData data = ManagerHub.Instance.ServerManager.ThisPlayerData;
            imageChange.Init(data.Class, data.HairStyleKey, data.SkinKey, data.FaceKey);
        }

        int hairStyleKey = ManagerHub.Instance.ServerManager.ThisPlayerData.HairStyleKey;
        int skinKey = ManagerHub.Instance.ServerManager.ThisPlayerData.SkinKey;
        int faceKey = ManagerHub.Instance.ServerManager.ThisPlayerData.FaceKey;

        ManagerHub.Instance.UIConnectManager.UICustomPanelManager.InitID(hairStyleKey, skinKey, faceKey);
        ManagerHub.Instance.UIConnectManager.UICustomPanelManager.ApplyPreview();


#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝날 때까지 대기");
#endif
        state?.SetLoadingBarValue(1f);
        await state?.TaskProgressBar;


#if MoveSceneDebug
        Debug.Log("loadingState 삭제");
#endif
        await runner.UnloadScene("LoadingScene");
    }
}
