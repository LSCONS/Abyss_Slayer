using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public void Init(CharacterClass character)
    {
        animationNum = 0;
        tempTime = maxtemp;
        DictAnimationState[WeaponTop] = PlayerManager.Instance.CharacterSpriteDicitonary[character].WeaponTop;
        DictAnimationState[ClothTop] = PlayerManager.Instance.CharacterSpriteDicitonary[character].ClothTop;
        DictAnimationState[HairTop] = PlayerManager.Instance.CharacterSpriteDicitonary[character].HairTop;
        DictAnimationState[ClothBottom] = PlayerManager.Instance.CharacterSpriteDicitonary[character].ClothBottom;
        DictAnimationState[HairBottom] = PlayerManager.Instance.CharacterSpriteDicitonary[character].HairBottom;
        DictAnimationState[Face] = PlayerManager.Instance.CharacterSpriteDicitonary[character].Face;
        DictAnimationState[Skin] = PlayerManager.Instance.CharacterSpriteDicitonary[character].Skin;
        DictAnimationState[WeaponBottom] = PlayerManager.Instance.CharacterSpriteDicitonary[character].WeaponBottom;
        SetLoopAnimation(AnimationState.Idle1, animationNum);
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
}
