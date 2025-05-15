using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeGameStateButton : UIButton
{
    [Header("전환하고 싶은 스테이트를 선택해 보아요!")]
    [SerializeField] ESceneName gameStartState;
    public void Awake()
    {
        gameObject.SetActive(true);
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeState);
    }


    private async void ChangeState()
    {
        await GameFlowManager.Instance.ChangeState(gameStartState);
    }

}
