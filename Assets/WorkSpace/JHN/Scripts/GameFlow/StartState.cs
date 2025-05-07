using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class StartState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override async Task OnEnter()
    {
        Debug.Log("StartState OnEnter");
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Start);

        await SoundManager.Instance.Init(EGameState.Start);
        SoundManager.Instance.PlayBGM(EGameState.Start, 1);

        await Task.CompletedTask;
    }

    public override async Task OnExit()
    {
        Debug.Log("StartState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Start);
        SoundManager.Instance.UnloadSoundsByState(EGameState.Start);
        await Task.CompletedTask;
    }

    public override async void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            await ChangeState(new IntroState());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            GameFlowManager.Instance.RpcServerSceneLoad(EGameState.Lobby);
        }
    }
}
