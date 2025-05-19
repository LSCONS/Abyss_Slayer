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

    // 헤어 인덱스
    private List<int> availableHairKeys = new();  // 클래스별로 허용되는 헤어 리스트
    private int hairIndex = 0;                    // 이 리스트에서 몇 번째인지 (0부터 시작)
    /// <summary>
    /// 최대 인덱스 구해주는 프로퍼티
    /// </summary>
    private int maxSkinId => DataManager.Instance.DictIntToDictStateToSkinColorSprite.Keys.Max();
    private int maxFaceId => DataManager.Instance.DictIntToDictStateToFaceColorSprite.Keys.Max();

    public override void Init()
    {
        base.Init();
        ConnectedButton();
    }

    public override void Open(params object[] args)
    {
        base.Open(args);

        InitClassBasedClothing();    // 클래스별로 기본 초기화 해줌
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

        applyButton.onClick.AddListener(() => ApplyPreview());
        skinLeftBtn.onClick.AddListener(() => ChangeSkinIndex(-1));
        skinRightBtn.onClick.AddListener(() => ChangeSkinIndex(+1));
        faceLeftBtn.onClick.AddListener(() => ChangeFaceIndex(-1));
        faceRightBtn.onClick.AddListener(() => ChangeFaceIndex(+1));
        hairLeftBtn.onClick.AddListener(() => ChangeHairIndex(-1));
        hairRightBtn.onClick.AddListener(() => ChangeHairIndex(+1));
    }

    /// <summary>
    /// 클래스에 따라 옷/무기/ 기본 커마 정보 초기화해줌
    /// </summary>
    private void InitClassBasedClothing()
    {
        CharacterClass selectedClass = PlayerManager.Instance.selectedCharacterClass;
        var dict = PlayerManager.Instance.CharacterSpriteDicitonary;

        if (!dict.TryGetValue(selectedClass, out var spriteData))
        {
            Debug.LogWarning($"[CustomPanelManager] 스프라이트 데이터 없음 {selectedClass}");
            return;
        }

        if (spritePreview == null) return;

        // 기존 딕셔너리 상태 제거
        spritePreview.DictAnimationState.Clear();

        // 복사해서 새로 넣기 - 깊은 복사
        spritePreview.DictAnimationState[spritePreview.ClothTop] = DeepCopy(spriteData.ClothTop);
        spritePreview.DictAnimationState[spritePreview.ClothBottom] = DeepCopy(spriteData.ClothBottom);
        spritePreview.DictAnimationState[spritePreview.WeaponTop] = DeepCopy(spriteData.WeaponTop);
        spritePreview.DictAnimationState[spritePreview.WeaponBottom] = DeepCopy(spriteData.WeaponBottom);

        // 지금 클래스의 커마 정보 가져옴
        var info = PlayerManager.Instance.PlayerCustomizationInfo;

        // hair 키 리스트 먼저 계산 (클래스별로 허용되는 헤어키 필터링)
        List<int> allowedColors = HairColorConfig.HairColorIndexByClass[selectedClass];
        availableHairKeys = DataManager.Instance.DictIntToDictStateToHairStyleTopSprite.Keys
            .Where(k => allowedColors.Contains(k % 100))
            .OrderBy(k => k)
            .ToList();

        // 기본값 설정
        skinId = ParseColorIndexFromName(spriteData.Data.SkinName);
        faceId = ParseColorIndexFromName(spriteData.Data.FaceName);
        hairIndex = 0;

        // 커스텀 데이터가 있다면 그 값으로 갱신
        if (info != null)
        {
            skinId = info.skinId;
            faceId = info.faceId;

            int currentHairKey = info.hairId;
            int foundIndex = availableHairKeys.IndexOf(currentHairKey);
            hairIndex = (foundIndex != -1) ? foundIndex : 0;
        }
    }

    /// <summary>
    /// 딕셔너리 깊은 복사 해주는 메서드
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private Dictionary<AnimationState, Sprite[]> DeepCopy(Dictionary<AnimationState, Sprite[]> source)
    {
        return source.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    /// <summary>
    /// 스프라이트 이름에서 컬러 인덱스로
    /// </summary>
    /// <param name="name">어떤 거 파싱</param>
    /// <returns></returns>
    private int ParseColorIndexFromName(string name)
    {
        // 예: face_c2, m4_c5_top, f1_c6_bot 등
        if (string.IsNullOrEmpty(name)) return 1;
        var parts = name.Split('_');
        foreach (var part in parts)
        {
            if (part.StartsWith("c") && int.TryParse(part.Substring(1), out int result))
            {
                return result;
            }
        }
        return 1;
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
    private void ChangeHairIndex(int delta)
    {
        if (availableHairKeys.Count == 0) return;

        hairIndex = Mathf.Clamp(hairIndex + delta, 0, availableHairKeys.Count - 1);
        int currentHairKey = availableHairKeys[hairIndex];
        hairNameText.text = $"Hair {currentHairKey}";
        UpdateHairPreview();
        UpdateButtonInteractable();
    }

    /// <summary>
    /// 버튼 활성화 갱신해주기
    /// </summary>
    private void UpdateButtonInteractable()
    {
        skinLeftBtn.interactable = skinId > 1;
        skinRightBtn.interactable = skinId < maxSkinId;

        faceLeftBtn.interactable = faceId > 1;
        faceRightBtn.interactable = faceId < maxFaceId;

        hairLeftBtn.interactable = hairIndex > 0;
        hairRightBtn.interactable = hairIndex < availableHairKeys.Count - 1;
    }

    /// <summary>
    /// 텍스트 초기화
    /// </summary>
    private void UpdateAllTexts()
    {
        skinNameText.text = $"Skin {skinId}";
        faceNameText.text = $"Face {faceId}";
        hairNameText.text = availableHairKeys.Count > 0 ? $"Hair {availableHairKeys[hairIndex]}" : "Hair -";
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
        UpdateHairPreview();
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
        if (availableHairKeys.Count == 0) return;

        int key = availableHairKeys[hairIndex];
        TrySetPreviewPart(spritePreview.HairTop, DataManager.Instance.DictIntToDictStateToHairStyleTopSprite, key, AnimationState.Idle1);
        TrySetPreviewPart(spritePreview.HairBottom, DataManager.Instance.DictIntToDictStateToHairStyleBottomSprite, key, AnimationState.Idle1);
        spritePreview.UpdatePreview();
    }


    /// <summary>
    /// DictAnimationState에 파츠 스프라이트 세팅해주기
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dict"></param>
    /// <param name="id"></param>
    /// <param name="state"></param>
    private void TrySetPreviewPart(Image target, Dictionary<int, Dictionary<AnimationState, Sprite[]>> dict, int id, AnimationState state)
    {
        if (!spritePreview.DictAnimationState.ContainsKey(target))
            spritePreview.DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

        if (dict.TryGetValue(id, out var stateDict))
        {
            spritePreview.DictAnimationState[target][state] = stateDict.TryGetValue(state, out var sprites) ? sprites : new Sprite[0];
        }
        else
        {
            spritePreview.DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>
            {
                { state, new Sprite[0] }
            };
        }
    }

    /// <summary>
    /// 커마 적용
    /// </summary>
    private void ApplyPreview()
    {
        if (availableHairKeys.Count == 0) return;

        int selectedHairKey = availableHairKeys[hairIndex];
        PlayerManager.Instance.SetCustomization(
            skinId,
            faceId,
            selectedHairKey
        );
    }
}

/// <summary>
/// 클래스별 허용 머리 색깔들 인덱스 추출해야됨
/// 미리 정해놓은 색으로
/// </summary>
public static class HairColorConfig
{
    public static readonly Dictionary<CharacterClass, List<int>> HairColorIndexByClass = new()
    {
        { CharacterClass.Mage,           new List<int> { 6 } },
        { CharacterClass.Tanker,         new List<int> { 10 } },
        { CharacterClass.MagicalBlader,  new List<int> { 4 } },
        { CharacterClass.Healer,         new List<int> { 2 } },
        { CharacterClass.Rogue,          new List<int> { 5 } },
    };
}
