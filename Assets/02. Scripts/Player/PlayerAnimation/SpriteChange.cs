using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.UI.Image;

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
    private List<Sprite> sortedFrames = new();

    public async void Init(PlayerSpriteData data)
    {
        await LoadAndSortSprites(WeaponTop, data.WeaponTopName);
        await LoadAndSortSprites(ClothTop, data.ClothTopName);
        await LoadAndSortSprites(HairTop, data.HairTopName);
        await LoadAndSortSprites(ClothBottom, data.ClothBottomName);
        await LoadAndSortSprites(HairBottom, data.HairBottomName);
        await LoadAndSortSprites(Face, data.FaceName);
        await LoadAndSortSprites(Skin, data.SkinName);
        await LoadAndSortSprites(WeaponBottom, data.WeaponBottomName);
    }
    // 스프라이트 로드한 다음에 정렬해줌
    private async System.Threading.Tasks.Task LoadAndSortSprites(SpriteRenderer renderer, string addressKey)
    {
        var handle = Addressables.LoadAssetAsync<Sprite[]>(addressKey);         // 우선 스프라이트 시트를 로드함 Sprite[]로 로드해서 스프라이트를 가져옴
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite[] spriteArray = handle.Result;
            sortedFrames = new List<Sprite>(spriteArray);

            // 스프라이트를 이름 숫자 부분으로 정렬함 (왜? addressable은 보장을 안해준대서)
            sortedFrames = spriteArray
                .OrderBy(sprite =>
                {
                    string name = sprite.name;
                    int number = int.Parse(name.Split('_').Last());
                    return number;
                }).ToList();
        }
        SetDictAnimationState(renderer);
    }


    // 애니메이션 스테이트 넣어주면 그 애니메이션을 재생해줌
    private void SetDictAnimationState(SpriteRenderer renderer)
    {
        if (sortedFrames == null) return;

        // SpriteSlicer로 정렬된 전체 시트(sortedFrames)를 애니메이션 상태별로 분리함
        var animationDict = SpriteSlicer.SliceSprite(sortedFrames.ToArray());

        DictAnimationState.Add(renderer, animationDict);
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

    public void SetSpriteCopy(SpriteChange change)
    {
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
