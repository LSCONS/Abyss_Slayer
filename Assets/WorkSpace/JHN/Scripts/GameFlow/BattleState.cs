using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
public class BattleState : BaseGameState
{
    public static int BossSceneCount { get; private set; } = 4;
    public override UIType StateUIType => UIType.GamePlay;

    public int stageIndex = 0;
    public bool isStart { get; set; } = false;
    private bool isBossDead { get; set; } = false; // 보스 죽음?
    private float deadTimer { get; set; } = 0.0f; // 보스 죽고 몇초 지남?
    private float changeSceneTime { get; set; } = 5.0f; // 보스 죽고 몇초 지나야 씬 넘어갈거임?

    public override Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Boss);
        await Task.CompletedTask;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!(isStart) || ServerManager.Instance.Boss == null) return;
       
        if (ServerManager.Instance.Boss.IsDead)
        {
            deadTimer += Time.deltaTime;
           // Debug.Log($"{deadTimer} 시간은 똑딱똑딱 {changeSceneTime} 까지");
            if (deadTimer >= changeSceneTime)
            {
                Debug.Log("보스 죽었다고 넘어가라고");

               GameFlowManager.Instance.GoToRestState();
            }
        }
    }

    public override async Task OnRunnerEnter()
    {
#if AllMethodDebug
        Debug.Log("BattleState OnRunnerEnter");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;
        isStart = false;
        stageIndex = ServerManager.Instance.BossCount;
        await UIManager.Instance.Init();
        state?.SetLoadingBarValue(0.3f);

        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        await ServerManager.Instance.WaitForDespawnBossAsync();

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        state?.SetLoadingBarValue(0.4f);

        await ServerManager.Instance.WaitForAllPlayerIsReady();
        state?.SetLoadingBarValue(0.5f);

        if (runner.IsServer)
            ServerManager.Instance.AllPlayerIsReadyFalse();

        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기

        // 보스 찾기

#if MoveSceneDebug
        Debug.Log("보스 소환할게요");
#endif
        if (runner.IsServer)
        {
            NetworkObject boss = runner.Spawn(DataManager.Instance.DictEnumToNetObjcet[EBossStage.Boss0],
                Vector3.right * 100,
                Quaternion.identity,
                ServerManager.Instance.ThisPlayerRef);
            runner.MoveGameObjectToScene(boss.gameObject, SceneRef.FromIndex((int)ESceneName.Battle0Scene));


            if(ServerManager.Instance.InitSupporter == null)
            {
                runner.Spawn(DataManager.Instance.InitSupporter);
            }
        }
        await ServerManager.Instance.WaitforBossSpawn();
        state?.SetLoadingBarValue(0.7f);

#if MoveSceneDebug
        Debug.Log("Rpc 래디 해주세용");
#endif
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();
        state?.SetLoadingBarValue(0.9f);

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }
        //TODO: 플레이어 위치 동기화도 필요함


#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await (state?.TaskProgressBar ?? Task.CompletedTask);


#if MoveSceneDebug
        Debug.Log("Battle 개방");
#endif
        UIManager.Instance.OpenUI(UISceneType.Boss);
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

#if MoveSceneDebug
            Debug.Log("보스 패턴 시작");
#endif
            ServerManager.Instance.Boss.BossController.StartBossPattern();

#if MoveSceneDebug
            Debug.Log("loadingState 삭제");
#endif
            await runner.UnloadScene(SceneName.LoadingScene);
        }

        isStart = true;
        isBossDead = false; // 씬 넘어가고 나면 다시 안타게 막아두자
        deadTimer = 0;
    }
}
