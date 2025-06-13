using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeGameStateButton : UIButton
{
    [Header("전환하고 싶은 스테이트를 선택해 보아요!")]
    [SerializeField] ESceneName gameStartState;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(true);
        button?.onClick.RemoveListener(ChangeState);
        button?.onClick.AddListener(ChangeState);
    }


    private async void ChangeState()
    {
        await ManagerHub.Instance.GameFlowManager.ChangeStateWithLoading(gameStartState);
    }

}
