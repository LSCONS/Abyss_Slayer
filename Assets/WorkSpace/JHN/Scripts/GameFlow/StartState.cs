using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class StartState : BaseGameState
{
    public override async Task OnEnter()
    {
        Debug.Log("StartState OnEnter");
        await LoadSceneManager.Instance.LoadScene(SceneName.StartScene);
        await UIManager.Instance.LoadAllUI(UIType.NonGamePlay);
        UIManager.Instance.CreateAllUI(UIType.NonGamePlay);
        UIManager.Instance.Init();
        UIManager.Instance.OpenUI(UISceneType.Start);

        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("StartState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Start);
        await Task.CompletedTask;
    }

    public override async void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            await ChangeState(new IntroState());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            await ChangeState(new LobbyState());
        }
    }
}
