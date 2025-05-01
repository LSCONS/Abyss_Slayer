using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class InGameState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;

    public int stageIndex;
    private Boss boss;  // 보스 체크해줘야됨
    private bool isBossDead = false; // 보스 죽음?
    private float deadTimer = 0.0f; // 보스 죽고 몇초 지남?
    private float changeSceneTime = 5.0f; // 보스 죽고 몇초 지나야 씬 넘어갈거임?
   
    public InGameState(int stageIndex)
    {
        this.stageIndex = stageIndex;
    }
    public override async Task OnEnter()
    {
        Debug.Log("InGameState OnEnter");
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기


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
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Boss);
       // UIManager.Instance.CleanupUIMap();


        // UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }

    public override async void OnUpdate()
    {
        base.OnUpdate();

        if (boss == null) return;
       
        if (boss.isDead)
        {
            deadTimer += Time.deltaTime;
           // Debug.Log($"{deadTimer} 시간은 똑딱똑딱 {changeSceneTime} 까지");
            if (deadTimer >= changeSceneTime)
            {
                Debug.Log("보스 죽었다고 넘어가라고");

               await GameFlowManager.Instance.GoToRestState();
            }
        }
    }
}
