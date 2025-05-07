using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeGameStateButton : UIButton
{
    [Header("전환하고 싶은 스테이트를 선택해 보아요!")]
    [SerializeField] EGameState gameStartState;
    public void Awake()
    {
        Debug.Log($"[UIChangeGameStateButton] Init 호출됨: {gameStartState}");
        gameObject.SetActive(true);
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeState);
        Debug.Log("이거 등록 안되는거에요?");
    }


    private async void ChangeState()
    {
        switch (gameStartState)
        {
            case EGameState.Intro:
                GameFlowManager.Instance.RpcServerSceneLoad(gameStartState);
                break;
            case EGameState.Start:
                GameFlowManager.Instance.RpcServerSceneLoad(gameStartState);
                break;
            case EGameState.Lobby:
                GameFlowManager.Instance.GoToLobby();
                break;
            case EGameState.Rest:
                GameFlowManager.Instance.GoToRestState();
                break;
            case EGameState.Battle:
                GameFlowManager.Instance.GoToNextBoss();
                break;
            default:
                Debug.LogWarning($"[UIChangeGameStateButton] 알 수 없는 상태: {gameStartState}");
                break;
        }
    }

}
