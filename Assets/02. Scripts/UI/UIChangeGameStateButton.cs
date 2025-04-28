using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeGameStateButton : UIButton
{
    [Header("전환하고 싶은 스테이트를 선택해 보아요!")]
    [SerializeField] GameStartState gameStartState;
    public override void Init()
    {
        base.Init();
        Debug.Log($"[UIChangeGameStateButton] Init 호출됨: {gameStartState}");
        gameObject.SetActive(true);
        button.onClick.RemoveListener(ChangeState);
        button.onClick.AddListener(ChangeState);
    }


    private async void ChangeState()
    {
        switch (gameStartState)
        {
            case GameStartState.Intro:
                await GameFlowManager.Instance.ChangeState(new IntroState());
                break;
            case GameStartState.Start:
                await GameFlowManager.Instance.ChangeState(new StartState());
                break;
            case GameStartState.Lobby:
                await GameFlowManager.Instance.GoToLobby();
                break;
            case GameStartState.Rest:
                await GameFlowManager.Instance.GoToRestState();
                break;
            case GameStartState.Boss:
                await GameFlowManager.Instance.GoToNextBoss();
                break;
            default:
                Debug.LogWarning($"[UIChangeGameStateButton] 알 수 없는 상태: {gameStartState}");
                break;
        }
    }

}
