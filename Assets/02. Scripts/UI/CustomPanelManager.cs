using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanelManager : UIPopup
{
    [Header("프리뷰 연결")]
    [SerializeField] private SpriteImageChange spritePreview;

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
    private int skinId = 1;
    private int faceId = 1;
    private int hairId = 1;

    /// <summary>
    /// 선택된 인덱스들을 적용해서 int id로 반환해줌
    /// </summary>
    private int maxSkinId => DataManager.Instance.DictIntToDictStateToSkinColorSprite.Keys.Max();
    private int maxFaceId => DataManager.Instance.DictIntToDictStateToFaceColorSprite.Keys.Max();
    private int maxHairId => DataManager.Instance.DictIntToDictStateToHairStyleTopSprite.Keys.Max();

    public override void Init()
    {
        base.Init();
        ConnectedButton();
        //UpdateAllTexts();
        //InitClassBasedClothing();
        //UpdatePreview();
    }

    public override void Open(params object[] args)
    {
        base.Open(args);
        InitClassBasedClothing();    // cloth/weapon
        UpdateAllTexts();            // 텍스트 초기화
        UpdatePreview();             // 커스텀 파츠 반영
        UpdateButtonInteractable();
    }

    // 버튼 연결
    private void ConnectedButton()
    {
        applyButton.onClick.RemoveAllListeners();
        skinLeftBtn.onClick.RemoveAllListeners();
        skinRightBtn.onClick.RemoveAllListeners();
        faceLeftBtn.onClick.RemoveAllListeners();
        faceRightBtn.onClick.RemoveAllListeners();
        hairLeftBtn.onClick.RemoveAllListeners();
        hairRightBtn.onClick.RemoveAllListeners();

        applyButton.onClick.AddListener(() => ApplyPreview());
        skinLeftBtn.onClick.AddListener(() => ChangeSkinIndex(-1));
        skinRightBtn.onClick.AddListener(() => ChangeSkinIndex(+1));
        faceLeftBtn.onClick.AddListener(() => ChangeFaceIndex(-1));
        faceRightBtn.onClick.AddListener(() => ChangeFaceIndex(+1));
        hairLeftBtn.onClick.AddListener(() => ChangeHairIndex(-1));
        hairRightBtn.onClick.AddListener(() => ChangeHairIndex(+1));
    }

    private void InitClassBasedClothing()
    {
        CharacterClass selectedClass = PlayerManager.Instance.GetSelectedClass();
        var dict = PlayerManager.Instance.CharacterSpriteDicitonary;

        if (!dict.TryGetValue(selectedClass, out var spriteData))
        {
            Debug.LogWarning($"[CustomPanelManager] 해당 클래스의 SpriteData를 찾을 수 없습니다: {selectedClass}");
            return;
        }

        if (spritePreview == null) return;

        spritePreview.DictAnimationState[spritePreview.ClothTop] = spriteData.ClothTop;
        spritePreview.DictAnimationState[spritePreview.ClothBottom] = spriteData.ClothBottom;
        spritePreview.DictAnimationState[spritePreview.WeaponTop] = spriteData.WeaponTop;
        spritePreview.DictAnimationState[spritePreview.WeaponBottom] = spriteData.WeaponBottom;
    }
    private void ChangeSkinIndex(int id)
    {
        skinId = Mathf.Clamp(skinId + id, 1, maxSkinId);
        skinNameText.text = $"Skin {skinId}";
        UpdateSkinPreview();
        UpdateButtonInteractable();
    }

    private void ChangeFaceIndex(int id)
    {
        faceId = Mathf.Clamp(faceId + id, 1, maxFaceId);
        faceNameText.text = $"Face {faceId}";
        UpdateFacePreview(); // 얼굴만 갱신
        UpdateButtonInteractable();
    }

    private void ChangeHairIndex(int id)
    {
        hairId = Mathf.Clamp(hairId + id, 1, maxHairId);
        hairNameText.text = $"Hair {hairId}";
        UpdateHairPreview(); // 헤어만 갱신
        UpdateButtonInteractable();
    }

    private void UpdateButtonInteractable()
    {
        skinLeftBtn.interactable = skinId > 1;
        skinRightBtn.interactable = skinId < maxSkinId;

        faceLeftBtn.interactable = faceId > 1;
        faceRightBtn.interactable = faceId < maxFaceId;

        hairLeftBtn.interactable = hairId > 1;
        hairRightBtn.interactable = hairId < maxHairId;
    }
    /// <summary>
    /// 시작 시 텍스트를 초기화
    /// </summary>
    private void UpdateAllTexts()
    {
        skinNameText.text = $"Skin {skinId}";
        faceNameText.text = $"Face {faceId}";
        hairNameText.text = $"Hair {hairId}";
    }

    /// <summary>
    /// 현재 선택된 옵션으로 프리뷰 갱신
    /// </summary>
    private void UpdatePreview()
    {
        if (spritePreview == null) return;

        var state = AnimationState.Idle1;

        TrySetPreviewPart(spritePreview.Skin, DataManager.Instance.DictIntToDictStateToSkinColorSprite, skinId, state);
        TrySetPreviewPart(spritePreview.Face, DataManager.Instance.DictIntToDictStateToFaceColorSprite, faceId, state);
        TrySetPreviewPart(spritePreview.HairTop, DataManager.Instance.DictIntToDictStateToHairStyleTopSprite, hairId, state);
        TrySetPreviewPart(spritePreview.HairBottom, DataManager.Instance.DictIntToDictStateToHairStyleBottomSprite, hairId, state);

        spritePreview.UpdatePreview();
    }

    private void UpdateSkinPreview()
    {
        TrySetPreviewPart(spritePreview.Skin, DataManager.Instance.DictIntToDictStateToSkinColorSprite, skinId, AnimationState.Idle1);
        spritePreview.UpdatePreview();
    }

    private void UpdateFacePreview()
    {
        TrySetPreviewPart(spritePreview.Face, DataManager.Instance.DictIntToDictStateToFaceColorSprite, faceId, AnimationState.Idle1);
        spritePreview.UpdatePreview();
    }

    private void UpdateHairPreview()
    {
        TrySetPreviewPart(spritePreview.HairTop, DataManager.Instance.DictIntToDictStateToHairStyleTopSprite, hairId, AnimationState.Idle1);
        TrySetPreviewPart(spritePreview.HairBottom, DataManager.Instance.DictIntToDictStateToHairStyleBottomSprite, hairId, AnimationState.Idle1);
        spritePreview.UpdatePreview();
    }

    private void TrySetPreviewPart(Image target, Dictionary<int, Dictionary<AnimationState, Sprite[]>> dict, int id, AnimationState state)
    {
        if (!spritePreview.DictAnimationState.ContainsKey(target))
            spritePreview.DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

        if (dict.TryGetValue(id, out var stateDict))
        {

            if (stateDict.TryGetValue(state, out var sprites))
            {
                spritePreview.DictAnimationState[target][state] = sprites;
            }
            else
            {
                // 예외 방지를 위해 빈 스프라이트 배열이라도 추가
                spritePreview.DictAnimationState[target][state] = new Sprite[0];
            }
        }
        else
        {
            // 예외 방지를 위해 전체 키 추가 및 빈 상태도 넣어줌
            spritePreview.DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>
        {
            { state, new Sprite[0] }
        };
        }
    }

    private void ApplyPreview()
    {
        PlayerManager.Instance.SetCustomization(
          skinId,
          faceId,
          hairId
      );
    }
}
