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
    private int SelectedClass = -1;
    public Dictionary<Image, Dictionary<AnimationState, Sprite[]>> DictAnimationState { get; set; } = new();
    private int animationNum = 0;
    private int maxtemp = 5;
    private int tempTime = 0;

    public void FixedUpdate()
    {
        if (DictAnimationState != null && tempTime == 0)
        {
            SetLoopAnimation(AnimationState.Idle1, ++animationNum);
            tempTime = maxtemp;
        }
        if (tempTime > 0)
        {
            tempTime--;
        }
    }


    public void Init(CharacterClass character, int hairStyleKey, int skinKey, int faceKey)
    {
        DataManager data = ManagerHub.Instance.DataManager;
        int hairColorKey = HairColorConfig.HairColorIndexByClass[character];
        SelectedClass = (int)character;
        (int, int) hairKey = (hairStyleKey, hairColorKey);
        animationNum = 0;
        tempTime = maxtemp;
        DictAnimationState[WeaponTop]       = data.InstantiateDictionary(data.DictClassToStateToWeaponTop[character]);
        DictAnimationState[ClothTop]        = data.InstantiateDictionary(data.DictClassToStateToClothTop[character]);
        DictAnimationState[ClothBottom]     = data.InstantiateDictionary(data.DictClassToStateToClothBot[character]);
        DictAnimationState[WeaponBottom]    = data.InstantiateDictionary(data.DictClassToStateToWeaponBot[character]);
        DictAnimationState[HairTop]         = data.InstantiateDictionary(data.DictIntToDictStateToHairStyleTopSprite[hairKey]);
        DictAnimationState[HairBottom]      = data.InstantiateDictionary(data.DictIntToDictStateToHairStyleBottomSprite[hairKey]);
        DictAnimationState[Face]            = data.InstantiateDictionary(data.DictIntToDictStateToFaceColorSprite[faceKey]);
        DictAnimationState[Skin]            = data.InstantiateDictionary(data.DictIntToDictStateToSkinColorSprite[skinKey]);
        foreach (var image in DictAnimationState.Keys)
        {
            image.color = Color.white;
        }
        SetLoopAnimation(AnimationState.Idle1, animationNum);
    }

    public bool SetOnceAnimation(AnimationState state, int spriteNum)
    {
        foreach (var image in DictAnimationState.Keys)
        {
            if (!DictAnimationState[image].ContainsKey(state))
                return false;

            var sprites = DictAnimationState[image][state];

            if (sprites == null || spriteNum >= sprites.Length)
                return false;

            image.sprite = sprites[spriteNum];
        }
        return true;
    }

    public void SetLoopAnimation(AnimationState state, int spriteNum)
    {
        foreach (var image in DictAnimationState.Keys)
        {
            if (!DictAnimationState[image].ContainsKey(state))
                continue;

            var sprites = DictAnimationState[image][state];

            if (sprites == null || sprites.Length == 0)
                continue; // 빈 배열이면 넘어감

            spriteNum %= sprites.Length;
            image.sprite = sprites[spriteNum];
        }
    }

    //public void ApplyCustomData(PlayerCustomizationInfo info)
    //{
    //    if (info == null) return;

    //    var data = ManagerHub.Instance.DataManager;
    //    var state = AnimationState.Idle1;
    //    var key = HairColorConfig.HairColorIndexByClass[PlayerManager.Instance.selectedCharacterClass];

    //    TrySetPart(Skin, data.DictIntToDictStateToSkinColorSprite, info.skinId, state);
    //    TrySetPart(Face, data.DictIntToDictStateToFaceColorSprite, info.faceId, state);
    //    var hairKey = (info.hairInfo.styleId, info.hairInfo.colorIndex);
    //    TrySetPart(HairTop, data.DictIntToDictStateToHairStyleTopSprite, hairKey, state);
    //    TrySetPart(HairBottom, data.DictIntToDictStateToHairStyleBottomSprite, hairKey, state);

    //    animationNum = 0; // 첫 프레임부터 재생
    //    SetLoopAnimation(state, animationNum);
    //}

    //private void TrySetPart(Image target, Dictionary<int, Dictionary<AnimationState, Sprite[]>> dict, int id, AnimationState state)
    //{
    //    if (!DictAnimationState.ContainsKey(target))
    //        DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

    //    if (dict.TryGetValue(id, out var stateDict) && stateDict.TryGetValue(state, out var sprites))
    //    {
    //        DictAnimationState[target][state] = sprites;
    //    }
    //}

    //private void TrySetPart(Image target, Dictionary<(int, int), Dictionary<AnimationState, Sprite[]>> dict, (int, int) id, AnimationState state)
    //{
    //    if (!DictAnimationState.ContainsKey(target))
    //        DictAnimationState[target] = new Dictionary<AnimationState, Sprite[]>();

    //    if (dict.TryGetValue(id, out var stateDict) && stateDict.TryGetValue(state, out var sprites))
    //    {
    //        DictAnimationState[target][state] = sprites;
    //    }
    //}

}
