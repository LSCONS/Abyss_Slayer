using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class SelectClassPanelController : UIPopup
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI descText;

    private CompositeDisposable disposables = new CompositeDisposable();
    private ClassSlotController controller;

    private CharacterClass selectedCharacterClass;
    public override void Init()
    {
        base.Init();
        controller = GetComponentInChildren<ClassSlotController>();
        if(!closeButton) closeButton = transform.GetGameObjectSameNameDFS("Close").GetComponent<Button>();
        if (!selectButton) selectButton = transform.GetGameObjectSameNameDFS("Select").GetComponent<Button>();


        if(controller.Slots.Count == 0) controller.CreateClassSlots();  // 클래스 슬롯 생성
    }

    public override void Open(params object[] args)
    {
        base.Open();
        closeButton.onClick.AddListener(OnClose);
        selectButton.onClick.AddListener(OnSelect);

        // class slot에서 발생한 이벤트를 구독하기
        foreach (var slot in controller.Slots)
        {
            slot.OnClickAsObservable
                .Subscribe(OnClassSelected)
                .AddTo(disposables);
        }
    }

    private void OnClassSelected(CharacterClass cc)
    {
        selectedCharacterClass = cc;

        // 설명 갱신
        descText.text = cc.GetDescription();
    }

    private void OnSelect()
    {
        PlayerManager.Instance.SetSelectedClass(selectedCharacterClass);
        OnClose();
    }

    public override void OnClose()
    {
        Debug.Log("OnClose");
        disposables.Clear();
        base.OnClose();

    }
}
