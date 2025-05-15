using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class RestState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public override async Task OnEnter()
    {
        Debug.Log("RestState OnEnter");
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Rest);

        // 브금 init
        await SoundManager.Instance.Init(ESceneName.Rest);

        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        UIManager.Instance.CloseUI(UISceneType.Rest);
        // UIManager.Instance.CleanupUIMap();
        // UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        Debug.Log("RestState OnEnter");
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Rest);
        await Task.CompletedTask;
        var runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer && await ServerManager.Instance.WaitForAllPlayerLoadingAsync())
        {
            ServerManager.Instance.ThisPlayerData.Rpc_ConnectInput();
        }
    }
}
