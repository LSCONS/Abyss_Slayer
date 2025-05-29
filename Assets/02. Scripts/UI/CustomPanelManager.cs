using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 커스터마이징 설정하는 팝업 UI (피부/얼굴/헤어 변경)
/// </summary>
public class CustomPanelManager : UIPopup
{
    [field: Header("프리뷰 연결")]
    [field: SerializeField] private SpriteImageChange SpritePreview { get; set; }

    // 적용하기 버튼
    [Header("적용하기 버튼")]
    [SerializeField] private Button applyButton;

    // 피부 ui       
    [Header("피부 UI")]
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private Button skinLeftBtn;
    [SerializeField] private Button skinRightBtn;

    // 얼굴 ui
    [Header("얼굴 UI")]
    [SerializeField] private TextMeshProUGUI faceNameText;
    [SerializeField] private Button faceLeftBtn;
    [SerializeField] private Button faceRightBtn;

    // 헤어 스타일 ui
    [Header("헤어 UI")]
    [SerializeField] private TextMeshProUGUI hairNameText;
    [SerializeField] private Button hairLeftBtn;
    [SerializeField] private Button hairRightBtn;

    // 지금 선택된 인덱스
    private int SkinId { get; set; } = 1;
    private int FaceId { get; set; } = 1;
    private int HairStyleID { get; set; } = 1;


    /// <summary>
    /// 최대 인덱스 구해주는 프로퍼티
    /// </summary>
    private int maxSkinId => DataManager.Instance.MaxSkinKey;
    private int maxFaceId => DataManager.Instance.MaxFaceKey;
    private int maxHairId => DataManager.Instance.MaxHairFKey + DataManager.Instance.MaxHairMKey;

    public override void Init()
    {
        base.Init();
        ConnectedButton();
        ServerManager.Instance.CustomPanelManager = this;
        gameObject.SetActive(false);
    }

    public override void Open(params object[] args)
    {
        base.Open(args);

        UpdateAllTexts();            // 텍스트 초기화
        UpdatePreview();             // 커스텀 파츠 반영
        UpdateButtonInteractable();  // 버튼 활성화
    }



    /// <summary>
    /// 버튼 초기화 후 연결
    /// </summary>
    private void ConnectedButton()
    {
        applyButton.onClick.RemoveAllListeners();
        skinLeftBtn.onClick.RemoveAllListeners();
        skinRightBtn.onClick.RemoveAllListeners();
        faceLeftBtn.onClick.RemoveAllListeners();
        faceRightBtn.onClick.RemoveAllListeners();
        hairLeftBtn.onClick.RemoveAllListeners();
        hairRightBtn.onClick.RemoveAllListeners();

        applyButton.onClick.AddListener(() => ClickApplyButton());
        skinLeftBtn.onClick.AddListener(() => ChangeSkinIndex(-1));
        skinRightBtn.onClick.AddListener(() => ChangeSkinIndex(+1));
        faceLeftBtn.onClick.AddListener(() => ChangeFaceIndex(-1));
        faceRightBtn.onClick.AddListener(() => ChangeFaceIndex(+1));
        hairLeftBtn.onClick.AddListener(() => ChangeHairIndex(-1));
        hairRightBtn.onClick.AddListener(() => ChangeHairIndex(+1));
    }


    private void ChangeSkinIndex(int id)
    {
#if AllMethodDebug
        Debug.Log("ChangeSkinIndex");
#endif
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        SkinId = Mathf.Clamp(SkinId + id, 1, maxSkinId);
        skinNameText.text = $"Skin {SkinId}";
        UpdatePreview();
        UpdateButtonInteractable();
    }

    private void ChangeFaceIndex(int id)
    {
#if AllMethodDebug
        Debug.Log("ChangeFaceIndex");
#endif
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        FaceId = Mathf.Clamp(FaceId + id, 1, maxFaceId);
        faceNameText.text = $"Face {FaceId}";
        UpdatePreview();
        UpdateButtonInteractable();
    }
    private void ChangeHairIndex(int id)
    {
#if AllMethodDebug
        Debug.Log("ChangeHairIndex");
#endif
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        HairStyleID = Mathf.Clamp(HairStyleID + id, 1, maxHairId);
        hairNameText.text = $"Hair {HairStyleID}";
        UpdatePreview();
        UpdateButtonInteractable();
    }

    /// <summary>
    /// 버튼 활성화 갱신해주기
    /// </summary>
    private void UpdateButtonInteractable()
    {
#if AllMethodDebug
        Debug.Log("UpdateButtonInteractable");
#endif
        skinLeftBtn.interactable = SkinId > 1;
        skinRightBtn.interactable = SkinId < maxSkinId;

        faceLeftBtn.interactable = FaceId > 1;
        faceRightBtn.interactable = FaceId < maxFaceId;

        hairLeftBtn.interactable = HairStyleID > 1;
        hairRightBtn.interactable = HairStyleID < maxHairId;
    }

    /// <summary>
    /// 텍스트 초기화
    /// </summary>
    private void UpdateAllTexts()
    {
#if AllMethodDebug
        Debug.Log("UpdateAllTexts");
#endif
        skinNameText.text = $"Skin {SkinId}";
        faceNameText.text = $"Face {FaceId}";
        hairNameText.text = $"Hair {HairStyleID}";
    }

    /// <summary>
    /// 현재 선택된 옵션으로 프리뷰 갱신
    /// </summary>
    private void UpdatePreview()
    {
#if AllMethodDebug
        Debug.Log("UpdatePreview");
#endif
        if (SpritePreview == null) return;
        SpritePreview.Init(ServerManager.Instance.ThisPlayerData.Class, HairStyleID, SkinId, FaceId);
    }

    /// <summary>
    /// 커마 적용
    /// </summary>
    public async void ApplyPreview()
    {
#if AllMethodDebug
        Debug.Log("ApplyPreview");
#endif
        await ServerManager.Instance.WaitForThisPlayerDataAsync();
        ServerManager.Instance.ThisPlayerData.Rpc_InitPlayerCustom(HairStyleID, SkinId, FaceId);
        OnClose();
    }


    private void ClickApplyButton()
    {
#if AllMethodDebug
        Debug.Log("ClickApplyButton");
#endif
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        ApplyPreview();
    }
}
