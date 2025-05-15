using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupButton : UIButton
{
    [Header("닫기 버튼으로 사용하고 싶다면 체크해주세요.")]
    [SerializeField] private bool isClose = false;  // 닫기 버튼으로 사용하고 싶으면 체크

    [Header("열고싶은 팝업 이름을 적어주세요.")]
    [SerializeField] private string popupName;  // 열고싶은 팝업 이름
    
    [Header("알람 아이콘 세팅")]
    [SerializeField] private GameObject alarmIcon;  // 알람 아이콘

    private UIPopup popup;

    public void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }

    public override void Init()
    {
        if (!isClose)
        {
            popup = UIManager.Instance.FindPopupByName(popupName);  // 닫기 기능 수행하기 싫어야지 popupname으로 찾아서 그거 열기
        }
        base.Init();
    }


    /// <summary>
    /// 알람 아이콘 세팅
    /// </summary>
    /// <param name="On">킬 건지</param>
    public void OnAlarmIcon(bool On)
    {
        if (alarmIcon == null) return;
        if(On ==  true) alarmIcon.SetActive(true);
        else alarmIcon.SetActive(false);
    }


    // 버튼 자동 연결 string으로 이름 받아서 그 이름의 버튼 찾아서 연결
    public void OnClickButton()
    {
        if (isClose)
        {
            // ui 팝업 스택에서 젤 위에 있는 팝업을 닫아줌
            if (UIManager.Instance.isPopupOpen<UIPopup>())  //  팝업 열려있는지 검사 후 열려있어야지 수행 (그래야 팝업 있는거니꺠)
            {
                
                var currentPopup = UIManager.Instance.popupStack.Peek();    // 젤 위에 있는거 꺼내옴
                UIManager.Instance.CloseCurrentPopup(currentPopup);
            }
            else
            {
                Debug.Log("[UIPopupBotton(OnClickButton)] 뭐임 지금 팝업이 아무것도 안열려있는데? 뭘...누른거야?");
            }
            return;
        }

        popup = UIManager.Instance.FindPopupByName(popupName);

        if (popup != null)
        {
            UIManager.Instance.OpenPopup(popup);
        }
        else
        {
            Debug.LogError($"[UIPopupBotton] 팝업 {popupName} 을 찾을 수 없다요");

        }
    }
}
