using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerManager : Singleton<PlayerManager>
{
    public Player Player { get; set; }
    public PlayerSpriteData PlayerSpriteData {  get; private set; }
    public PlayerCustomizationInfo PlayerCustomizationInfo { get; private set; }

    public CharacterClass selectedCharacterClass { get; set; } = CharacterClass.Rogue;
    // 커스텀 바뀌면 콜백
    public Action<PlayerCustomizationInfo> OnCustomizationChanged { get; set; }

    public CharacterClass CharacterClass
        => ServerManager.Instance.ThisPlayerData?.Class ?? CharacterClass.Rogue;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }
    

    /// <summary>
    /// 현재 씬에서 플레이어를 찾고 등록하는 메서드
    /// 플레이어가 있는 씬으로 이동할 때마다 호출해야함.
    /// </summary>
    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedClass)
    {
        selectedCharacterClass = selectedClass;
        ServerManager.Instance.ThisPlayerData.Rpc_ChangeClass(selectedClass);
    }

    public void SetCustomization(int skinId, int faceId, (int style, int color) hairId)
    {
        ServerManager.Instance.ThisPlayerData.Rpc_InitPlayerCustom(hairId.style, hairId.color, skinId, faceId);
    }
}

public class SpriteData
{
    [field: SerializeField] public PlayerSpriteData Data { get; set; } = new PlayerSpriteData();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> WeaponTop { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> ClothTop { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> HairTop { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> ClothBottom { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> HairBottom { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> Face { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> Skin { get; set; } = new();
    [field: SerializeField] public Dictionary<AnimationState, Sprite[]> WeaponBottom { get; set; } = new();





    public async Task Init(CharacterClass playerClass)
    {
        Data.SetSpriteName(playerClass);
        WeaponTop       = await LoadAndSortSprites(Data.WeaponTopName);
        ClothTop        = await LoadAndSortSprites(Data.ClothTopName);
        ClothBottom     = await LoadAndSortSprites(Data.ClothBottomName);
        WeaponBottom    = await LoadAndSortSprites(Data.WeaponBottomName);
    }


    private async Task<Dictionary<AnimationState, Sprite[]>> LoadAndSortSprites(string addressKey)
    {
        var handle = Addressables.LoadAssetAsync<Sprite[]>(addressKey);         // 우선 스프라이트 시트를 로드함 Sprite[]로 로드해서 스프라이트를 가져옴
        await handle.Task;
        List<Sprite> sortedFrames = null;
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
        // SpriteSlicer로 정렬된 전체 시트(sortedFrames)를 애니메이션 상태별로 분리함
        return SpriteSlicer.SliceSprite(sortedFrames.ToArray());
    }

}
