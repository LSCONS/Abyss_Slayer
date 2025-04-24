using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupBotton : UIBase
{
    [Header("열고싶은 팝업 이름을 적어주세요.")]
    [SerializeField] private string popupName;  // 열고싶은 팝업 이름
    private UIPopup popup;
    private Button button;
    private void Awake() 
    {
        button = GetComponent<Button>();
    }
    public override void Init()
    {
        popup = UIManager.Instance.FindPopupByName(popupName);
        OnClickButton();
        base.Init();
        gameObject.SetActive(true);

        Debug.Log("버튼 init됐나요?");
    }

    // 버튼 자동 연결 string으로 이름 받아서 그 이름의 버튼 찾아서 연결
    public void OnClickButton()
    {
        button.onClick.AddListener(() =>
        {
            if (popup == null)
            {
                popup = UIManager.Instance.FindPopupByName(popupName);
            }

            if (popup != null)
            {
                UIManager.Instance.OpenPopup(popup);
            }
            else
            {
                Debug.LogError($"[UIPopupBotton] 팝업 {popupName} 을 찾을 수 없다요");
            }
        });
    }

}
