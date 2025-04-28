using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class RestState : BaseGameState
{
    private int stageIndex;
    public RestState(int stageIndex)
    {
        this.stageIndex = stageIndex;   
    }
    public override async Task OnEnter()
    {
        Debug.Log("RestState OnEnter");
        await LoadSceneManager.Instance.LoadScene(SceneName.RestScene);
        await UIManager.Instance.LoadAllUI(UIType.GamePlay);
        UIManager.Instance.CreateAllUI(UIType.GamePlay);
        UIManager.Instance.Init();
        UIManager.Instance.CloseAllPermanent();
        UIManager.Instance.CloseAllPopup();

        UIManager.Instance.OpenUI(UISceneType.Rest);
    }

    public override async Task OnExit()
    {
        UIManager.Instance.ClearUI(UIType.GamePlay);            // 게임 플레이 UI 제거
        await Task.CompletedTask;
    }
}
