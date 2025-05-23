using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;
public class EndingState : BaseGameState
{
    public override UIType StateUIType => UIType.GamePlay;
    public Vector3 StartPosition { get; private set; } = new Vector3(-18, 1.5f, 0);
    public override ESceneName SceneName => ESceneName.EndingScene;

    public override Task OnEnter()
    {
        return Task.CompletedTask;
    }

    public override async Task OnExit()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnExit 실행");
#endif
        UIManager.Instance.CloseUI(UISceneType.Rest);
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            ServerManager.Instance.ThisPlayerData.Rpc_DisconnectInput();
        }

        await Task.CompletedTask;
    }

    public override async Task OnRunnerEnter()
    {
#if MoveSceneDebug
        Debug.Log("RestState OnRunnerEnter 실행");
#endif
        LoadingState state = GameFlowManager.Instance.prevLodingState;

        state?.SetLoadingBarValue(0.3f);

#if MoveSceneDebug
        Debug.Log("서버에서 보스 스폰 실행");
#endif
        var runner = RunnerManager.Instance.GetRunner();

#if MoveSceneDebug
        Debug.Log("서버야 모든 데이터가 유효하니");
#endif
        if (runner.IsServer)
        {
            //모든 플레이어의 데이터가 들어있는지 확인하는 메서드
            await ServerManager.Instance.WaitForAllPlayerLoadingAsync();
        }
        state?.SetLoadingBarValue(1);
        await state?.TaskProgressBar;
        await UIManager.Instance.Init();

        SoundManager.Instance.PlayBGM(EAudioClip.BGM_EndingScene);
        ServerManager.Instance.ThisPlayerData.Rpc_SetReady(true);
        await ServerManager.Instance.WaitForAllPlayerIsReady();
        if (runner.IsServer)
        {
#if MoveSceneDebug
        Debug.Log("loadingState 삭제");
#endif
            await runner.UnloadScene("LoadingScene");
        }

        GameFlowManager.Instance.endCredit.StartScrollCredit();
    }

    // 크레딧 끝나면 다음 스테이트로 넘어갈 수 있도록 대기
    private async Task WaitForCreditEnd()
    {
        CreditRoller creditObj = GameFlowManager.Instance.endCredit;

        if (creditObj == null) return;

        // 크래딧 끝날 때까지 기다림
        await creditObj.CreditEndTask;

        // TODO: 로비로 보내주세요
      
    }
}
