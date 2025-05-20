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
        UIManager.Instance.CloseUI(UISceneType.Rest);
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnRunnerEnter 실행");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;

        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.3f);



#if MoveSceneDebug
        Debug.Log("서버에서 보스 스폰 실행");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            if(PoolManager.Instance == null)
            {
                PoolManager a = runner.Spawn(DataManager.Instance.PoolManagerPrefab);
            }


            NetworkObject boss = runner.Spawn
                (
                DataManager.Instance.DictEnumToNetObjcet[EBossStage.Rest],
                new Vector3(5, 6.5f, 0),
                Quaternion.identity,
                ServerManager.Instance.ThisPlayerRef
            )
            ;
            runner.MoveGameObjectToScene(boss.gameObject, SceneRef.FromIndex((int)ESceneName.RestScene));
        }
        await ServerManager.Instance.WaitforBossSpawn();
        //TODO: 보스 씬 옮겨야함
        //TODO: 보스 위치 옮겨야함

        state?.SetLoadingBarValue(0.6f);

#if MoveSceneDebug
        Debug.Log("Rpc 래디 해주세용");
#endif
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();
        state?.SetLoadingBarValue(0.7f);

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            ServerManager.Instance.AllPlayerIsReadyFalse();
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }

#if MoveSceneDebug
        Debug.Log("RestState 개방");
#endif
        UIManager.Instance.OpenUI(UISceneType.Rest);

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();

        //플레이어 시작 위치 값 초기화
        if (runner.IsServer)
        {
            Vector3 temp = StartPosition;
            foreach(Player player in ServerManager.Instance.DictRefToPlayer.Values)
            {
                player.PlayerPosition = temp;
                temp += Vector3.right;
            }

            foreach(NetworkData data in ServerManager.Instance.DictRefToNetData.Values)
            {
                data.Rpc_ResetPlayerPosition();
            }
        }

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;

        if (runner.IsServer)
        {
#if MoveSceneDebug
        Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif
            ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();

#if MoveSceneDebug
        Debug.Log("1초만 기다려줘");
#endif
            await Task.Delay(100);
            ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();

#if MoveSceneDebug
        Debug.Log("loadingState 삭제");
#endif
            await runner.UnloadScene("LoadingScene");
        }
    }
}
