using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class InGameState : BaseGameState
{
    private int stageIndex;
    public InGameState(int stageIndex)
    {
        this.stageIndex = stageIndex;
    }
    public override async Task OnEnter()
    {
        Debug.Log("InGameState OnEnter");
        LoadSceneManager.Instance.LoadScene("TestScene2");     // 씬 로드 (일단 BossScene1)

        UIManager.Instance.ClearUI(UIType.NonGamePlay);         // 비게임 플레이 UI 제거
        UIManager.Instance.CreateAllUI(UIType.GamePlay);       // 게임 플레이 UI 생성
        UIManager.Instance.OpenUI(UISceneType.Boss);       // 게임 플레이 UI 열기
        UIManager.Instance.Init();                
        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }
}
