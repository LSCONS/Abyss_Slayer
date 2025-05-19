using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class SpriteImageChange : MonoBehaviour
{
    [field: Header("연결할 스프라이트")]
    [field: SerializeField] public Image WeaponTop { get; set; }
    [field: SerializeField] public Image ClothTop { get; set; }
    [field: SerializeField] public Image HairTop { get; set; }
    [field: SerializeField] public Image ClothBottom { get; set; }
    [field: SerializeField] public Image HairBottom { get; set; }
    [field: SerializeField] public Image Face { get; set; }
    [field: SerializeField] public Image Skin { get; set; }
    [field: SerializeField] public Image WeaponBottom { get; set; }
    [field: SerializeField] public SpriteData  SpriteData { get; set; }
    private Color color = new Color(1,1,1,1);
    private int SelectedClass = -1;
    public Dictionary<Image, Dictionary<AnimationState, Sprite[]>> DictAnimationState { get; set; } = new();
    private int animationNum = 0;
    private int maxtemp = 5;
    private int tempTime = 0;

    public void FixedUpdate()
    {
        if(DictAnimationState != null && tempTime == 0)
        {
            SetLoopAnimation(AnimationState.Idle1, ++animationNum);
            tempTime = maxtemp;
        }
        if(tempTime > 0)
        {
            tempTime--;
        }
    }

    private void Awake()
    {
        // 액션 등록
        PlayerManager.Instance.OnCustomizationChanged += ApplyCustomData;
    }
    public void Init(CharacterClass character)
    {
        if(SelectedClass != (int)character)
        {
            SelectedClass = (int)character;
            animationNum = 0;
            tempTime = maxtemp;
            DictAnimationState[WeaponTop] = PlayerManager.Instance.DictClassToSpriteData[character].WeaponTop;
            DictAnimationState[ClothTop] = PlayerManager.Instance.DictClassToSpriteData[character].ClothTop;
            DictAnimationState[HairTop] = PlayerManager.Instance.DictClassToSpriteData[character].HairTop;
            DictAnimationState[ClothBottom] = PlayerManager.Instance.DictClassToSpriteData[character].ClothBottom;
            DictAnimationState[HairBottom] = PlayerManager.Instance.DictClassToSpriteData[character].HairBottom;
            DictAnimationState[Face] = PlayerManager.Instance.DictClassToSpriteData[character].Face;
            DictAnimationState[Skin] = PlayerManager.Instance.DictClassToSpriteData[character].Skin;
            DictAnimationState[WeaponBottom] = PlayerManager.Instance.DictClassToSpriteData[character].WeaponBottom;
            foreach (var image in DictAnimationState.Keys)
            {
                image.color = color;
            }
            SetLoopAnimation(AnimationState.Idle1, animationNum);
        }
    }

    public bool SetOnceAnimation(AnimationState state, int spriteNum)
    {
        foreach (var spriteRenderer in DictAnimationState.Keys)
        {
            if (!DictAnimationState[spriteRenderer].ContainsKey(state))
                return false;

            var sprites = DictAnimationState[spriteRenderer][state];

            if (sprites == null || spriteNum >= sprites.Length)
                return false;

            spriteRenderer.sprite = sprites[spriteNum];
        }
        return true;
    }

    public void SetLoopAnimation(AnimationState state, int spriteNum)
    {
        foreach (var spriteRenderer in DictAnimationState.Keys)
        {
            if (!DictAnimationState[spriteRenderer].ContainsKey(state))
                continue;

            var sprites = DictAnimationState[spriteRenderer][state];

            if (sprites == null || sprites.Length == 0)
                continue; // 빈 배열이면 넘어감

            spriteNum %= sprites.Length;
            spriteRenderer.sprite = sprites[spriteNum];
        }
    }

    public void ApplyCustomData(PlayerCustomizationInfo info)
    {
        if (info == null) return;

        var data = DataManager.Instance;
        var state = AnimationState.Idle1;
        var key = HairColorConfig.HairColorIndexByClass[PlayerManager.Instance.selectedCharacterClass];

        TrySetPart(Skin, data.DictIntToDictStateToSkinColorSprite, info.skinId, state);
        TrySetPart(Face, data.DictIntToDictStateToFaceColorSprite, info.faceId, state);
        TrySetPart(HairTop, data.DictIntToDictStateToHairStyleTopSprite, info.hairId, state);
        TrySetPart(HairBottom, data.DictIntToDictStateToHairStyleBottomSprite, info.hairId, state);

        animationNum = 0; // 첫 프레임부터 재생
        SetLoopAnimation(state, animationNum);
    }

    private void TrySetPart(Image target, Dictionary<int, Dictionary<AnimationState, Sprite[]>> dict, int id, AnimationState state)
    {
        if (!DictAnimationState.ContainsKey(target))
            DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

        if (dict.TryGetValue(id, out var stateDict) && stateDict.TryGetValue(state, out var sprites))
        {
            DictAnimationState[target][state] = sprites;
        }
    }

    private void TrySetPart(Image target, Dictionary<(int, int), Dictionary<AnimationState, Sprite[]>> dict, (int, int) id, AnimationState state)
    {
        if (!DictAnimationState.ContainsKey(target))
            DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

        if (dict.TryGetValue(id, out var stateDict) && stateDict.TryGetValue(state, out var sprites))
        {
            DictAnimationState[target][state] = sprites;
        }
    }
    public void UpdatePreview()
    {
        animationNum = 0;
        SetLoopAnimation(AnimationState.Idle1, animationNum);
    }

}
