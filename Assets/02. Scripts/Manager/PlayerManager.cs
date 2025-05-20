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
    public Dictionary<CharacterClass, SpriteData> CharacterSpriteDicitonary { get; set; } = new();

    // 커스텀 바뀌면 콜백
    public Action<PlayerCustomizationInfo> OnCustomizationChanged;

    public CharacterClass CharacterClass
        => ServerManager.Instance.ThisPlayerData?.Class ?? CharacterClass.Rogue;
    public Dictionary<CharacterClass, SpriteData> DictClassToSpriteData { get; private set; } = new();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Sprite Data 생성
        LoadSpriteData();
    }



    private async void LoadSpriteData()
    {
        try
        {
            foreach (CharacterClass character in Enum.GetValues(typeof(CharacterClass)))
            {
                if (character == CharacterClass.Count) continue;

                DictClassToSpriteData[character] = new SpriteData();
                await DictClassToSpriteData[character].Init(character);

                CharacterSpriteDicitonary[character] = DictClassToSpriteData[character];
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SpriteData초기화 실패: {ex}");
        }

        if (PlayerCustomizationInfo == null)
        {
            CharacterClass defaultClass = selectedCharacterClass;

            int defaultSkin = 0;
            int defaultFace = 0;
            (int, int) defaultHair = (0, HairColorConfig.HairColorIndexByClass[defaultClass]);

            PlayerCustomizationInfo = new PlayerCustomizationInfo(defaultSkin, defaultFace, defaultHair);
        }
    }


    private int ParseColorIndexFromName(string name)
    {
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

    private string ExtractStyleIdTokenFromName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "m1";
        var parts = name.Split('_');
        foreach (var part in parts)
        {
            if ((part.StartsWith("m") || part.StartsWith("f")) && part.Length > 1)
                return part;
        }
        return "m1";
    }

    private int CreateHairKey(string styleId, int colorIndex)
    {
        int baseOffset = styleId.StartsWith("m") ? 1000 : 4000;
        int styleNum = int.Parse(styleId.Substring(1));
        return baseOffset + styleNum * 100 + colorIndex;
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
        PlayerCustomizationInfo = new PlayerCustomizationInfo(skinId, faceId, hairId);
        OnCustomizationChanged?.Invoke(PlayerCustomizationInfo);
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


    public Dictionary<AnimationState, Sprite[]> InstantiateDictionary(Dictionary<AnimationState, Sprite[]> data)
    {
        Dictionary<AnimationState, Sprite[]> result = new(); 
        foreach(AnimationState state in data.Keys)
        {
            Sprite[] sprites = new Sprite[data[state].Length];
            for(int i = 0; i < data[state].Length; i++)
            {
                sprites[i] = data[state][i];
            }
            result[state] = sprites;
        }
        return result;
    }


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
