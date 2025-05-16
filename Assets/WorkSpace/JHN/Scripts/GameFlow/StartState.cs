using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class StartState : BaseGameState
{
    public override UIType StateUIType => UIType.NonGamePlay;
    public override async Task OnEnter()
    {
        Debug.Log("StartState OnEnter");
        UIManager.Instance.Init();

        await SoundManager.Instance.Init(ESceneName.Start);
        SoundManager.Instance.PlayBGM(ESceneName.Start, 1);

        //서버에 연결
        var temp = ServerManager.Instance.ConnectRoomSearch();
        await temp;

        LoadingState state = GameFlowManager.Instance.prevLodingState;
        if (state != null)
        {
            state.IsLoadFast = true;
            await state.TaskProgressBar;
        }

        UIManager.Instance.OpenUI(UISceneType.Start);
        SceneManager.UnloadSceneAsync(SceneName.LoadingScene);

        return;
    }

    public override async Task OnExit()
    {
        Debug.Log("StartState OnExit");
        UIManager.Instance.CloseUI(UISceneType.Start);
        SoundManager.Instance.UnloadSoundsByState(ESceneName.Start);
        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
        Debug.Log("StartState OnEnter");
        UIManager.Instance.Init();

        UIManager.Instance.OpenUI(UISceneType.Start);

        SoundManager.Instance.Init(ESceneName.Start);
        SoundManager.Instance.PlayBGM(ESceneName.Start, 1);

        await Task.CompletedTask;
    }
}
