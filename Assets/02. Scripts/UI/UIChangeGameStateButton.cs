using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeGameStateButton : UIButton
{
    [Header("전환하고 싶은 스테이트를 선택해 보아요!")]
    [SerializeField] EGameState gameStartState;
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
            case EGameState.Intro:
                await GameFlowManager.Instance.ChangeState(new IntroState());
                break;
            case EGameState.Start:
                await GameFlowManager.Instance.ChangeState(new StartState());
                break;
            case EGameState.Lobby:
                await GameFlowManager.Instance.GoToLobby();
                break;
            case EGameState.Rest:
                await GameFlowManager.Instance.GoToRestState();
                break;
            case EGameState.Battle:
                await GameFlowManager.Instance.GoToNextBoss();
                break;
            default:
                Debug.LogWarning($"[UIChangeGameStateButton] 알 수 없는 상태: {gameStartState}");
                break;
        }
    }

}
