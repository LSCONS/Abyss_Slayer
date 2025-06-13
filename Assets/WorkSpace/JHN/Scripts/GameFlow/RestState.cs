using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;
public class RestState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public Vector3 StartPosition { get; private set; } = new Vector3(-18, 1.5f, 0);
    public override ESceneName SceneName => ESceneName.RestScene;
    public override Task OnEnter()
    {
        return Task.CompletedTask;
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
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ResetRestButton();
        }
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnRunnerEnter 실행");
#endif
        LoadingState state = ManagerHub.Instance.GameFlowManager.prevLodingState;

        await ManagerHub.Instance.UIManager.UIInit();
        state?.SetLoadingBarValue(0.3f);

        ManagerHub.Instance.UIConnectManager.UIPlayerState.UIHealthBar.ConnectPlayerObject(await ManagerHub.Instance.ServerManager.WaitForThisPlayerAsync());
        await Task.Yield();
        ManagerHub.Instance.UIManager.ResetAllRectTransform();
#if MoveSceneDebug
        Debug.Log("서버에서 보스 스폰 실행");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();
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
            runner.MoveGameObjectToScene(boss.gameObject, SceneRef.FromIndex((int)ESceneName.RestScene));
        }
        await ManagerHub.Instance.ServerManager.WaitforBossSpawn();

        state?.SetLoadingBarValue(0.6f);

#if MoveSceneDebug
        Debug.Log("Rpc 래디 해주세용");
#endif
        state?.SetLoadingBarValue(0.7f);

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            await ManagerHub.Instance.ServerManager.WaitForAllPlayerLoadingAsync();
        }

#if MoveSceneDebug
        Debug.Log("RestState 개방");
#endif
        ManagerHub.Instance.SoundManager.PlayBGM(EAudioClip.BGM_RestScene);
        ManagerHub.Instance.UIManager.OpenUI(UISceneType.Rest);

        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetReady(true);
        await ManagerHub.Instance.ServerManager.WaitForAllPlayerIsReadyTrue();
        await Task.Delay(60);
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetReady(false);
        await ManagerHub.Instance.ServerManager.WaitForAllPlayerIsReadyFalse();
        await Task.Delay(60);


        //플레이어 시작 위치 값 초기화
        if (runner.IsServer)
        {
#if MoveSceneDebug
            Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();
            //UI텍스트 초기화
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetInRestTeamText();

            Vector3 temp = StartPosition;
            foreach (Player player in ManagerHub.Instance.ServerManager.DictRefToPlayer.Values)
            {
                await player.PlayerPositionReset(temp);
                temp += Vector3.right;
            }
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_PlayerActiveTrue();
        }
        else
        {
            //클라이언트들은 휴식 씬 들어오면 기본적으로 준비 버튼을 활성화 하도록 함.
            ManagerHub.Instance.UIConnectManager.UIReadyBossStage?.SetActiveButton(true);
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
            ManagerHub.Instance.ServerManager.AllPlayerIsReadyFalse();
        }
    }
}
