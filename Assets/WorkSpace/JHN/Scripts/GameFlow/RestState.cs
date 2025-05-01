using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class RestState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;

    private int stageIndex;
    public RestState(int stageIndex)
    {
        this.stageIndex = stageIndex;   
    }
    public override async Task OnEnter()
    {
        Debug.Log("RestState OnEnter");
        PlayerManager.Instance.FindPlayer();
        PlayerManager.Instance.PlayerOnConnected();
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Rest);
        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Rest);
        // UIManager.Instance.CleanupUIMap();
        // UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }
}
