using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Fusion;
public class BattleState : BaseGameState
{
    public static int BossSceneCount { get; private set; } = 4;
    public override UIType StateUIType => UIType.GamePlay;
    public override ESceneName SceneName => (ESceneName)((int)ESceneName.BattleScene + stageIndex);
    public Vector3 StartPosition { get; private set; } = new Vector3(-18, 1.5f, 0);

    public int stageIndex => GameFlowManager.Instance.CurrentStageIndex;
    public bool isStart { get; set; } = false;
    private float deadTimer { get; set; } = 0.0f; // 보스 죽고 몇초 지남?
    private float changeSceneTime { get; set; } = 5.0f; // 보스 죽고 몇초 지나야 씬 넘어갈거임?

    // 퍼널 스텝 전송 플래그
    private bool sentHP100 = false;
    private bool sentHP66 = false;
    private bool sentHP33 = false;
    private bool sentHP5 = false;
    private bool sentStageClear = false;

    public override Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Boss);
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            ServerManager.Instance.ThisPlayerData.Rpc_DisconnectInput();
        }
        await Task.CompletedTask;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!(isStart) || ServerManager.Instance.Boss == null) return;

        var boss = ServerManager.Instance.Boss;
        float hpPercent = boss.Hp.Value / boss.MaxHp.Value * 100f;

        // 100%
        if (!sentHP100 && Mathf.Approximately(hpPercent, 100f))
        {
            sentHP100 = true;
            AnalyticsManager.SendFunnelStep(GetFunnelStepHP(100));
        }
        // 66%
        if (!sentHP66 && hpPercent <= 66f)
        {
            sentHP66 = true;
            AnalyticsManager.SendFunnelStep(GetFunnelStepHP(66));
        }
        // 33%
        if (!sentHP33 && hpPercent <= 33f)
        {
            sentHP33 = true;
            AnalyticsManager.SendFunnelStep(GetFunnelStepHP(33));
        }
        // 5%
        if (!sentHP5 && hpPercent <= 5f)
        {
            sentHP5 = true;
            AnalyticsManager.SendFunnelStep(GetFunnelStepHP(5));
        }

        // 보스 사망(스테이지 클리어)
        if (!sentStageClear && boss.IsDead)
        {
            sentStageClear = true;
            AnalyticsManager.SendFunnelStep(GetFunnelStepClear());
        }

        if (boss.IsDead)
        {
            deadTimer += Time.deltaTime;
            if (deadTimer >= changeSceneTime)
            {
                GameFlowManager.Instance.RpcServerSceneLoad(ESceneName.RestScene);
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
            NetworkObject boss = runner.Spawn(DataManager.Instance.DictEnumToNetObjcet[(EBossStage)GameFlowManager.Instance.CurrentStageIndex],
                Vector3.right * 100,
                Quaternion.identity,
                ServerManager.Instance.ThisPlayerRef);
            runner.MoveGameObjectToScene(boss.gameObject, runner.GetSceneRef(GameFlowManager.Instance.GetSceneNameFromState(this)));


            if(ServerManager.Instance.InitSupporter == null)
            {
                runner.Spawn(DataManager.Instance.InitSupporterPrefab);
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
            ServerManager.Instance.AllPlayerIsReadyFalse();
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }
        //TODO: 플레이어 위치 동기화도 필요함

#if MoveSceneDebug
        Debug.Log("Battle 개방");
#endif
        UIManager.Instance.OpenUI(UISceneType.Boss);

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();

        //플레이어 시작 위치 값 초기화
        if (runner.IsServer)
        {
            Vector3 temp = StartPosition;
            foreach (Player player in ServerManager.Instance.DictRefToPlayer.Values)
            {
                player.PlayerPosition = temp;
                temp += Vector3.right;
            }

            foreach (NetworkData data in ServerManager.Instance.DictRefToNetData.Values)
            {
                data.Rpc_ResetPlayerPosition();
            }
        }

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await (state?.TaskProgressBar ?? Task.CompletedTask);


        if (runner.IsServer)
        {
#if MoveSceneDebug
            Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif
            ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();

            // 모든 준비가 끝나고, 스테이지 입장 직후
            int memberCount = ServerManager.Instance.DictRefToPlayer.Count;

            var playerClasses = new string[5];
            int idx = 0;
            foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
            {
                playerClasses[idx] = player.NetworkData.Class.ToString();
                idx++;
                if (idx >= 5) break;
            }
            for (; idx < 5; idx++) playerClasses[idx] = "";

            // string difficulty = ServerManager.Instance.Difficulty;

            GameStartAnalytics.SendStartUserInfo(
                memberCount,
                // difficulty,
                playerClasses[0], playerClasses[1], playerClasses[2], playerClasses[3], playerClasses[4]
            );

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
            await runner.UnloadScene("LoadingScene");
        }

        isStart = true;
        deadTimer = 0;
    }

    // 퍼널 스텝 번호 반환 함수들
    private int GetFunnelStepHP(int percent)
    {
        switch (stageIndex)
        {
            case 0: // 스테이지1
                if (percent == 100) return 4;
                if (percent == 66) return 5;
                if (percent == 33) return 6;
                if (percent == 5) return 7;
                break;
            case 1: // 스테이지2
                if (percent == 100) return 11;
                if (percent == 66) return 12;
                if (percent == 33) return 13;
                if (percent == 5) return 14;
                break;
            case 2: // 스테이지3
                if (percent == 100) return 18;
                if (percent == 66) return 19;
                if (percent == 33) return 20;
                if (percent == 5) return 21;
                break;
        }
        return -1;
    }

    private int GetFunnelStepClear()
    {
        switch (stageIndex)
        {
            case 0: return 8;   // 스테이지1 클리어
            case 1: return 15;  // 스테이지2 클리어
            case 2: return 22;  // 스테이지3 클리어
        }
        return -1;
    }
}
