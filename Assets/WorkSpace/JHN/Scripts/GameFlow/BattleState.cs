using Fusion;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class BattleState : BaseGameState
{
    public static int BossSceneCount { get; private set; } = 4;
    public override UIType StateUIType => UIType.GamePlay;
    public override ESceneName SceneName => ESceneName.BattleScene;
    public Vector3 StartPosition { get; private set; } = new Vector3(-18, 1.5f, 0);

    public int stageIndex => GameValueManager.Instance.CurrentStageIndex;
    public bool isStart { get; set; } = false;
    private float deadTimer { get; set; } = 0.0f; // 보스 죽고 몇초 지남?
    private float changeSceneTime { get; set; } = 5.0f; // 보스 죽고 몇초 지나야 씬 넘어갈거임?
    private float changeEndingSceneTime { get; set; } = 10.0f; // 마지막 보스 죽고 몇 초 지나야 씬 넘어갈거임?


    // 퍼널 스텝 전송 플래그
    private bool sentHP90 = false;
    private bool sentHP66 = false;
    private bool sentHP33 = false;
    private bool sentHP5 = false;
    private bool sentStageClear = false;
    private bool sentStageFail = false;

    private float stageStartTime = 0f;
    private float previousHpPercent = 100f;

    private int lastStageIndex = -1;

    public override Task OnEnter()
    {
        Debug.LogAssertion($"[BattleState] OnEnter 호출! stageIndex={stageIndex}");
        stageStartTime = Time.time;
        ResetFunnelFlags();
        previousHpPercent = 100f;
        lastStageIndex = stageIndex;
        return Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        UIManager.Instance.CloseUI(UISceneType.Boss);
        try
        {
            if(runner.IsServer) PoolManager.Instance.ReturnPoolAllObject();
            PoolManager.Instance.CrossHairObject.gameObject.SetActive(false);
        }
        catch { }
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
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        var boss = ServerManager.Instance.Boss;
        float hpPercent = (float)boss.Hp.Value / (float)boss.MaxHp.Value * 100f;

        if (lastStageIndex != stageIndex)
        {
            ResetFunnelFlags();
            lastStageIndex = stageIndex;
        }

        TrySendHpFunnelStep(ref sentHP90, hpPercent, 90f);
        TrySendHpFunnelStep(ref sentHP66, hpPercent, 66f);
        TrySendHpFunnelStep(ref sentHP33, hpPercent, 33f);
        TrySendHpFunnelStep(ref sentHP5,  hpPercent, 5f);

        previousHpPercent = hpPercent;

        int stageElapsedTime = (int)(Time.time - stageStartTime);

        // 1. 플레이어 전멸 체크
        if (!sentStageFail && AllPlayersDead())
        {
            sentStageFail = true;
            // 실패 데이터 전송
            StageAnalytics.SendStageFailInfo(
                stageNumber: (stageIndex + 1).ToString(),
                failTime: stageElapsedTime,
                player1Class: GetPlayerClass(0),
                player2Class: GetPlayerClass(1),
                player3Class: GetPlayerClass(2),
                player4Class: GetPlayerClass(3),
                player5Class: GetPlayerClass(4)
            );
        }

        // 2. 보스 사망 체크
        if (!sentStageClear && boss.IsDead)
        {
            sentStageClear = true;
            // 성공 데이터 전송
            StageAnalytics.SendStageClearInfo(
                stageNumber: (stageIndex + 1).ToString(),
                clearTime: stageElapsedTime,
                player1Class: GetPlayerClass(0), player1Damage: GetPlayerDamage(0), player1Death: GetPlayerDeath(0),
                player2Class: GetPlayerClass(1), player2Damage: GetPlayerDamage(1), player2Death: GetPlayerDeath(1),
                player3Class: GetPlayerClass(2), player3Damage: GetPlayerDamage(2), player3Death: GetPlayerDeath(2),
                player4Class: GetPlayerClass(3), player4Damage: GetPlayerDamage(3), player4Death: GetPlayerDeath(3),
                player5Class: GetPlayerClass(4), player5Damage: GetPlayerDamage(4), player5Death: GetPlayerDeath(4)
            );

            // 퍼널 스텝: 스테이지 클리어
            if (stageIndex == 0)
                AnalyticsManager.SendFunnelStep(8);  // 스테이지 1 클리어
            else if (stageIndex == 1)
                AnalyticsManager.SendFunnelStep(15); // 스테이지 2 클리어
        }

        if (boss.IsDead)
        {
            deadTimer += Time.deltaTime;

            // 마지막 보스인지 체크
            bool isFinalBoss = (GameValueManager.Instance.MaxBossStageCount - GameValueManager.Instance.CurrentStageIndex == 1);
            float sceneDelay = isFinalBoss ? changeEndingSceneTime : changeSceneTime;

            if (runner.IsServer && isFinalBoss && ServerManager.Instance.fireworks == null)
            {
                ServerManager.Instance.ThisPlayerData.Rpc_SetActiveTrueFireWorks();
            }


            if (deadTimer >= sceneDelay)
            {
                if (runner.IsServer)
                {

                    //모든 보스를 모두 처치한 상태일 경우
                    if(isFinalBoss)
                    {
                        //클리어한 모드가 하드 일 경우
                        if(GameValueManager.Instance.EGameLevel == EGameLevel.Hard)
                        {
                            //여러 사람과 플레이 했을 때
                            if(runner.SessionInfo.PlayerCount > 1)
                            {
                                ServerManager.Instance.ThisPlayerData.Rpc_MultiClearTime(Util.GetNowTimeStirng().StringToBytes(), runner.SessionInfo.PlayerCount);
                            }
                            else//혼자서 플레이 했을 때
                            {
                                PlayerPrefs.SetString(PlayerPrefabData.SoloClearTime, $"[{ServerManager.Instance.PlayerName}]\n{Util.GetNowTimeStirng()}");
                                ServerManager.Instance.UIStartTitle?.SoloClearTimeUpdate();
                            }
                        }
                        ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.EndingScene);
                    }
                    else
                    {
                        ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.RestScene);
                    }
                }
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
        await ServerManager.Instance.WaitForAllPlayerIsReadyTrue();
        await Task.Delay(60);
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(false);

        state?.SetLoadingBarValue(0.5f);

        if (runner.IsServer && PoolManager.Instance.CrossHairObject == null)
        {
            runner.Spawn(DataManager.Instance.CrossHairPrefab);
        }
        bool isFinalBoss = (GameValueManager.Instance.MaxBossStageCount - GameValueManager.Instance.CurrentStageIndex == 1);
            if (isFinalBoss) SoundManager.Instance.PlayBGM(EAudioClip.BGM_BattleScene_Last);
            else SoundManager.Instance.PlayBGM(EAudioClip.BGM_BattleScene);
        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기

        // 보스 찾기

#if MoveSceneDebug
        Debug.Log("보스 소환할게요");
#endif
        if (runner.IsServer)
        {
            NetworkObject boss = runner.Spawn
            (
            DataManager.Instance.DictEnumToBossObjcet[(EBossStage)GameValueManager.Instance.CurrentStageIndex],
            Vector3.right * 100,
            Quaternion.identity,
            ServerManager.Instance.ThisPlayerRef
            );
            runner.MoveGameObjectToScene(boss.gameObject, runner.GetSceneRef(GameFlowManager.Instance.GetSceneNameFromState(this)));
            //UI초기화
            ServerManager.Instance.ThisPlayerData.Rpc_SetBattleTeamText();

            if (ServerManager.Instance.InitSupporter == null)
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
        await ServerManager.Instance.WaitForAllPlayerIsReadyTrue();
        await Task.Delay(60); ;

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(false);
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
        Debug.Log("Battle 개방");
#endif
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReadyTrue();
        await Task.Delay(60); ;

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(false);

        //플레이어 시작 위치 값 초기화
        if (runner.IsServer)
        {
#if MoveSceneDebug
            Debug.Log("모든 플레이어 활성화 하고 입력 연결해줄게");
#endif

            Vector3 temp = StartPosition;
            foreach (Player player in ServerManager.Instance.DictRefToPlayer.Values)
            {
                await player.PlayerPositionReset(temp);
                temp += Vector3.right;
            }
            ServerManager.Instance.ThisPlayerData.Rpc_PlayerActiveTrue();
        }

#if MoveSceneDebug
        Debug.Log("프로그래스 바 끝났는지 확인하자");
#endif
        state?.SetLoadingBarValue(1);
        await (state?.TaskProgressBar ?? Task.CompletedTask);

        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);

        if (runner.IsServer)
        {

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

            await ServerManager.Instance.WaitForAllPlayerIsReadyTrue();
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


    // 모든 플레이어가 죽었는지 체크
    private bool AllPlayersDead()
    {
        foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
        {
            if (player.Hp.Value > 0)
                return false;
        }
        return true;
    }

    // 플레이어 클래스/데미지/사망 횟수 가져오는 함수 예시
    private string GetPlayerClass(int idx)
    {
        var players = ServerManager.Instance.DictRefToPlayer.Values.ToList();
        if (idx < players.Count)
            return players[idx].NetworkData.Class.ToString();
        return "";
    }
    private int GetPlayerDamage(int idx)
    {
        // 실제 데미지 집계 로직에 맞게 구현 필요
        return 0;
    }
    private int GetPlayerDeath(int idx)
    {
        // 실제 사망 횟수 집계 로직에 맞게 구현 필요
        return 0;
    }

    private void ResetFunnelFlags()
    {
        sentHP90 = false;
        sentHP66 = false;
        sentHP33 = false;
        sentHP5 = false;
        sentStageClear = false;
        sentStageFail = false;
    }
    

    // 퍼널 스텝 번호 반환 함수들
    private int GetFunnelStepHP(int percent)
    {
        int stepNumber = -1;
        switch (stageIndex)
        {
            case 0: // 스테이지1
                if (percent == 90) stepNumber = 4;
                if (percent == 66) stepNumber = 5;
                if (percent == 33) stepNumber = 6;
                if (percent == 5) stepNumber = 7;
                break;
            case 1: // 스테이지2
                if (percent == 90) stepNumber = 11;
                if (percent == 66) stepNumber = 12;
                if (percent == 33) stepNumber = 13;
                if (percent == 5) stepNumber = 14;
                break;
        }
        return stepNumber;
    }


    private void TrySendHpFunnelStep(ref bool sentFlag, float hpPercent, float threshold)
    {
        if (!sentFlag && hpPercent <= threshold)
        {
            sentFlag = true;
            int stepNumber = GetFunnelStepHP((int)threshold);
            
            try
            {
                if (!AnalyticsManager.IsInitialized) return;
                AnalyticsManager.SendFunnelStep(stepNumber);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{threshold}% 퍼널 스텝 전송 실패: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
