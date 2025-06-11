using Fusion;
using System.Threading.Tasks;
using UnityEngine;
public class TutorialState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public Vector3 StartPosition { get; private set; } = new Vector3(-18, 1.5f, 0);
    public override ESceneName SceneName => ESceneName.TutorialScene;
    public async override Task OnEnter()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnRunnerEnter 실행"); 
#endif
        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;

        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.3f);

        await ManagerHub.Instance.ServerManager.InitTutorial();
        await ManagerHub.Instance.ServerManager.WaitForThisPlayerDataAsync();
        ManagerHub.Instance.ServerManager.InstantiatePlayer();
        await ManagerHub.Instance.ServerManager.WaitForThisPlayerAsync();
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_MoveScene(ESceneName.TutorialScene);
        RunnerManager.Instance.GetRunner().SessionInfo.IsOpen = false;

#if MoveSceneDebug
        Debug.Log("서버에서 보스 스폰 실행");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            if (ManagerHub.Instance.ServerManager.PoolManager == null)
            {
                ManagerHub.Instance.ServerManager.PoolManager = runner.Spawn(ManagerHub.Instance.DataManager.PoolManagerPrefab);
            }


            NetworkObject boss = runner.Spawn
            (
                ManagerHub.Instance.DataManager.DictEnumToBossObjcet[EBossStage.Rest],
                new Vector3(5, 6.5f, 0),
                Quaternion.identity,
                ManagerHub.Instance.ServerManager.ThisPlayerRef
            )
            ;
            runner.MoveGameObjectToScene(boss.gameObject, SceneRef.FromIndex((int)ESceneName.TutorialScene));
        }
        await ManagerHub.Instance.ServerManager.WaitforBossSpawn();
        //TODO: 보스 씬 옮겨야함
        //TODO: 보스 위치 옮겨야함

        state?.SetLoadingBarValue(0.6f);

#if MoveSceneDebug
        Debug.Log("Rpc 래디 해주세용");
#endif
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetReady(true);
        await ManagerHub.Instance.ServerManager.WaitForAllPlayerIsReadyTrue();
        state?.SetLoadingBarValue(0.7f);

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();
            await ManagerHub.Instance.ServerManager.WaitForAllPlayerLoadingAsync();
        }

#if MoveSceneDebug
        Debug.Log("RestState 개방");
#endif
        ManagerHub.Instance.SoundManager.PlayBGM(EAudioClip.BGM_RestScene);
        ManagerHub.Instance.UIManager.OpenUI(UISceneType.Tutorial);

        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetReady(true);
        await ManagerHub.Instance.ServerManager.WaitForAllPlayerIsReadyTrue();

        //플레이어 시작 위치 값 초기화
        if (runner.IsServer)
        {
#if MoveSceneDebug
            Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();

            Vector3 temp = StartPosition;
            foreach (Player player in ManagerHub.Instance.ServerManager.DictRefToPlayer.Values)
            {
                await player.PlayerPositionReset(temp);
                temp += Vector3.right;
            }
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_PlayerActiveTrue();
        }

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;

        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetReady(true);
        if (runner.IsServer)
        {

#if MoveSceneDebug
            Debug.Log("1초만 기다려줘");
#endif
            await ManagerHub.Instance.ServerManager.WaitForAllPlayerIsReadyTrue();
            await Task.Delay(100);
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ConnectInput();

#if MoveSceneDebug
            Debug.Log("loadingState 삭제");
#endif
            await runner.UnloadScene("LoadingScene");
        }
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnExit 실행");
#endif
        ManagerHub.Instance.UIManager.CloseUI(UISceneType.Rest);
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            try
            {
                PoolManager.Instance.ReturnPoolAllObject();
            }
            catch { }
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_DisconnectInput();
        }

        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        await Task.CompletedTask;
    }
}
