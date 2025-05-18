using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInputName : UIPermanent
{
    [field: SerializeField] public Button BtnCreateName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextError { get; private set; }
    [field: SerializeField] public TMP_InputField InputNameField { get; private set; }
    private string PlayerName { get; set; } = "";

    private void Awake()
    {
        InputNameField.onValueChanged.AddListener(CheckNameError);
        BtnCreateName.onClick.AddListener(CreateName);
        BtnCreateName.interactable = false;
        gameObject.SetActive(true);
    }

    private void CheckNameError(string text)
    {
        string temp = text.Trim();

        if(temp.Length >= 2 && temp.Length <= 8)
        {
            TextError.text = "생성할 수 있는 이름입니다";
            TextError.color = Color.black;
            PlayerName = text;
            BtnCreateName.interactable = true;
        }
        else
        {
            TextError.text = "2 ~ 8자 이내로 입력해 주세요.";
            TextError.color = Color.red;
            BtnCreateName.interactable = false;
        }
    }

    private void CreateName()
    {
        //이름 저장
        ServerManager.Instance.PlayerName = PlayerName;
        //씬 이동
        GameFlowManager.Instance.ClientSceneLoad(ESceneName.StartScene);
    }
}
