using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class UIGameScene : MonoBehaviour
{
    [SerializeField] private Button settingsButton;

    private async void Awake()
    {
        await UIManager.Instance.LoadPopup<UIPopupSettings>("UIPopupSettings");
    }
    private void Start()
    {
        // 설정창 버튼 설정
        settingsButton.onClick.AddListener(()=>
        {
            UIManager.Instance.OpenPopup<UIPopupSettings>("UIPopupSettings");
        });
    }   
}