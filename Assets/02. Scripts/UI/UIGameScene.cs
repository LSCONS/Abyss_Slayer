using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGameScene : MonoBehaviour
{
    [SerializeField] private Button settingsButton;

    private void Start()
    {
        // 설정창 버튼 설정
        settingsButton.onClick.AddListener(async()=>
        {
            await UIManager.Instance.OpenPopupAsyncToName<UIPopupSettings>("UIPopupSettings");
        });
    }   
}