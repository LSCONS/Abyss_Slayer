using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    [field: Header("연결할 스프라이트")]
    [field: SerializeField] public SpriteRenderer WeaponTop { get; set; }
    [field: SerializeField] public SpriteRenderer ClothTop { get; set; }
    [field: SerializeField] public SpriteRenderer HairTop { get; set; }
    [field: SerializeField] public SpriteRenderer ClothBottom { get; set; }
    [field: SerializeField] public SpriteRenderer HairBottom { get; set; }
    [field: SerializeField] public SpriteRenderer Face { get; set; }
    [field: SerializeField] public SpriteRenderer Skin { get; set; }
    [field: SerializeField] public SpriteRenderer WeaponBottom { get; set; }
    public Dictionary<SpriteRenderer, Dictionary<AnimationState, Sprite[]>> DictAnimationState { get; set; } = new();

    public void Init(CharacterClass character, int hairStyleKey, int faceKey, int skinKey)
    {
        DataManager data = DataManager.Instance;
        int hairColotKey = HairColorConfig.HairColorIndexByClass[character];
        (int, int) hairKey = (hairStyleKey, hairColotKey);
        DictAnimationState[WeaponTop]       = data.InstantiateDictionary(data.DictClassToStateToWeaponTop[character]);
        DictAnimationState[ClothTop]        = data.InstantiateDictionary(data.DictClassToStateToClothTop[character]);
        DictAnimationState[HairTop]         = data.InstantiateDictionary(data.DictIntToDictStateToHairStyleTopSprite[hairKey]);
        DictAnimationState[ClothBottom]     = data.InstantiateDictionary(data.DictClassToStateToClothBot[character]);
        DictAnimationState[HairBottom]      = data.InstantiateDictionary(data.DictIntToDictStateToHairStyleBottomSprite[hairKey]);
        DictAnimationState[Face]            = data.InstantiateDictionary(data.DictIntToDictStateToFaceColorSprite[faceKey]);
        DictAnimationState[Skin]            = data.InstantiateDictionary(data.DictIntToDictStateToSkinColorSprite[skinKey]);
        DictAnimationState[WeaponBottom]    = data.InstantiateDictionary(data.DictClassToStateToWeaponBot[character]);
    }

    public bool SetOnceAnimation(AnimationState state, int spriteNum)
    {
        foreach (var spriteRenderer in DictAnimationState.Keys)
        {
            if (DictAnimationState[spriteRenderer][state].Length <= spriteNum) return false;
            spriteRenderer.sprite = DictAnimationState[spriteRenderer][state][spriteNum];
        }
        return true;
    }

    public void SetLoopAnimation(AnimationState state, int spriteNum)
    {
        foreach (var spriteRenderer in DictAnimationState.Keys)
        {
            spriteNum %= DictAnimationState[spriteRenderer][state].Length;
            spriteRenderer.sprite = DictAnimationState[spriteRenderer][state][spriteNum];
        }
    }

    public void SetFlipxSpriteRenderer(bool flipX)
    {
        foreach (var spriteRenderer in DictAnimationState.Keys)
        {
            spriteRenderer.flipX = flipX;
        }
    }

    public void SetSpriteColor(Color color)
    {
        WeaponTop.color = color;
        ClothTop.color = color;
        HairTop.color = color;
        ClothBottom.color = color;
        HairBottom.color = color;
        Face.color = color;
        Skin.color = color;
        WeaponBottom.color = color;
    }

    public void SetSpriteColorSilhouette(Color color)
    {
        WeaponTop.color = color;
        ClothTop.color = color * 2;
        HairTop.color = color;
        ClothBottom.color = color * 2;
        HairBottom.color = color;
        Face.color = color;
        Skin.color = color;
        WeaponBottom.color = color;
    }

    public void SetSpriteCopy(SpriteChange change)
    {

        WeaponTop.sortingOrder = (change.WeaponTop.sortingOrder);
        ClothTop.sortingOrder = (change.ClothTop.sortingOrder);
        HairTop.sortingOrder = (change.HairTop.sortingOrder);
        ClothBottom.sortingOrder = (change.ClothBottom.sortingOrder);
        HairBottom.sortingOrder = (change.HairBottom.sortingOrder);
        Face.sortingOrder = (change.Face.sortingOrder);
        Skin.sortingOrder = (change.Skin.sortingOrder);
        WeaponBottom.sortingOrder = (change.WeaponBottom.sortingOrder);

        WeaponTop.sprite = (change.WeaponTop.sprite);
        ClothTop.sprite = (change.ClothTop.sprite);
        HairTop.sprite = (change.HairTop.sprite);
        ClothBottom.sprite = (change.ClothBottom.sprite);
        HairBottom.sprite = (change.HairBottom.sprite);
        Face.sprite = (change.Face.sprite);
        Skin.sprite = (change.Skin.sprite);
        WeaponBottom.sprite = (change.WeaponBottom.sprite);
    }

    public void SetFlipXCopy(bool flipX)
    {
        WeaponTop.flipX = flipX;
        ClothTop.flipX = flipX;
        HairTop.flipX = flipX;
        ClothBottom.flipX = flipX;
        HairBottom.flipX= flipX;
        Face.flipX = flipX;
        Skin.flipX = flipX;
        WeaponBottom.flipX = flipX;
    }
}
