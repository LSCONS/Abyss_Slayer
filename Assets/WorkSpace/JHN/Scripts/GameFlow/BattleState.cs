using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BattleState : BaseGameState
{
    public static int BossSceneCount { get; private set; } = 4;
    public override UIType StateUIType => UIType.GamePlay;

    public int stageIndex = 0;
    private Boss boss;  // 보스 체크해줘야됨
    private bool isBossDead { get; set; } = false; // 보스 죽음?
    private float deadTimer = 0.0f; // 보스 죽고 몇초 지남?
    private float changeSceneTime = 5.0f; // 보스 죽고 몇초 지나야 씬 넘어갈거임?
    private float timeLimit = 300f; // 5분 제한 시간
    private float currentTime = 0f;

    public override Task OnEnter()
    {
        stageIndex = ServerManager.Instance.BossCount;
        Debug.Log("InGameState OnEnter");
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기

        // 브금 init
        await SoundManager.Instance.Init(ESceneName.Battle0);

        // 스테이지 시작 시간 기록
        AnalyticsManager.SendStartUserInfo(ServerManager.Instance.DictRefToPlayer.Count);

        // 보스 찾기
        var bossObj = GameObject.FindWithTag("Boss");
        if (bossObj != null)
        {
            boss = bossObj.GetComponent<Boss>();
            if(boss == null)
            {
                Debug.LogError("보스 컴포넌트 없더");
                return;
            }
        }
        else
        {
            Debug.LogError("보스 오브젝트 없어");
            return;
        }

        isBossDead = false; // 씬 넘어가고 나면 다시 안타게 막아두자
        deadTimer = 0;
        currentTime = 0f;
        return;
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

        if (boss == null) return;
       
        if (boss.IsDead)
        {
            if (!isBossDead)
            {
                isBossDead = true;
                // 보스 사망 시 클리어 애널리틱스 전송
                int survivingMembers = 0;
                foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
                {
                    if (player.Hp.Value > 0)
                    {
                        survivingMembers++;
                    }
                }

                AnalyticsManager.SendStageFailInfo(stageIndex, (int)currentTime);
            }

            deadTimer += Time.deltaTime;
            // Debug.Log($"{deadTimer} 시간은 똑딱똑딱 {changeSceneTime} 까지");
            if (deadTimer >= changeSceneTime)
            {
                Debug.Log("보스 죽었다고 넘어가라고");
                GameFlowManager.Instance.GoToRestState();
            }
        }
        else
        {
            // 제한 시간 체크
            currentTime += Time.deltaTime;
            if (currentTime >= timeLimit)
            {
                // 제한 시간 초과로 게임오버
                AnalyticsManager.SendStageFailInfo(stageIndex, (int)currentTime);
                GameFlowManager.Instance.RpcServerSceneLoad(ESceneName.Lobby);
            }
        }
    }

    public override Task OnRunnerEnter()
    {
        stageIndex = ServerManager.Instance.BossCount;
        Debug.Log("InGameState OnEnter");
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기


        // 보스 찾기
        var bossObj = GameObject.FindWithTag("Boss");
        if (bossObj != null)
        {
            boss = bossObj.GetComponent<Boss>();
            if (boss == null)
            {
                Debug.LogError("보스 컴포넌트 없더");
                return Task.CompletedTask;
            }
        }
        else
        {
            Debug.LogError("보스 오브젝트 없어");
            return Task.CompletedTask;
        }

        isBossDead = false; // 씬 넘어가고 나면 다시 안타게 막아두자
        deadTimer = 0;
        currentTime = 0f;
        return Task.CompletedTask;
    }
}
