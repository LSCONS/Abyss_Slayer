using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SelectClassPanelController : UIPopup
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] public SpriteImageChange spriteImageChange;

    private CompositeDisposable disposables = new CompositeDisposable();
    private ClassSlotController controller;

    private CharacterClass selectedCharacterClass;

    private void Awake()
    {
        controller = GetComponentInChildren<ClassSlotController>();
        if (closeButton == null) closeButton = transform.GetGameObjectSameNameDFS("Close").GetComponent<Button>();
        if (selectButton == null) selectButton = transform.GetGameObjectSameNameDFS("Select").GetComponent<Button>();
        closeButton.onClick.AddListener(OnClose);
        selectButton.onClick.AddListener(OnSelect);
        if (controller.Slots.Count == 0) controller.CreateClassSlots();  // 클래스 슬롯 생성
    }

    public override void Open(params object[] args)
    {
#if AllMethodDebug
        Debug.Log("Open");
#endif
        base.Open();

        // class slot에서 발생한 이벤트를 구독하기
        foreach (var slot in controller.Slots)
        {
            slot.OnClickAsObservable
                .Subscribe(OnClassSelected)
                .AddTo(disposables);
        }

        if(controller.Slots.Count > 0)  // 일다 ㄴ열면 첫번째 슬롯 자동 선택되어있도록
        {
            OnClassSelected(controller.Slots[0].CharacterClass);
        }
    }

    private void OnClassSelected(CharacterClass cc)
    {
#if AllMethodDebug
        Debug.Log("OnClassSelected");
#endif
        selectedCharacterClass = cc;
        // 설명 갱신
        descText.text = ManagerHub.Instance.DataManager.DictClassToCharacterData[selectedCharacterClass].Description;
        int hairStyleKey = 1;
        int hairColorkey = HairColorConfig.HairColorIndexByClass[cc];
        int skinkey = 1;
        int facekey = 1;

        if(uISceneType == UISceneType.Lobby)
        {
            hairStyleKey = ManagerHub.Instance.ServerManager.ThisPlayerData.HairStyleKey;
            skinkey = ManagerHub.Instance.ServerManager.ThisPlayerData.SkinKey;
            facekey = ManagerHub.Instance.ServerManager.ThisPlayerData.FaceKey;
        }


        spriteImageChange.Init(cc, hairStyleKey, skinkey, facekey);
    }

    private void OnSelect()
    {
#if AllMethodDebug
        Debug.Log("OnSelect");
#endif        
        if(uISceneType == UISceneType.Lobby)
        {
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ChangeClass(selectedCharacterClass);
        }
        else if (uISceneType == UISceneType.Start)
        {
            ManagerHub.Instance.GameFlowManager.ClientSceneLoad(ESceneName.TutorialScene);
        }
        OnClose();
    }

    public override void OnClose()
    {
#if AllMethodDebug
        Debug.Log("OnClose");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        disposables.Clear();
        base.OnClose();
    }
}
