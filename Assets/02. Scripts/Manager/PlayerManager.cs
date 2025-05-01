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

    public CharacterClass selectedCharacterClass;
    public Dictionary<CharacterClass, SpriteData> CharacterSpriteDicitonary { get; set; } = new();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        try
        {
            foreach (CharacterClass character in Enum.GetValues(typeof(CharacterClass)))
            {
                if (character == CharacterClass.Count) continue;

                CharacterSpriteDicitonary[character] = new SpriteData();
                await CharacterSpriteDicitonary[character].Init(character);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"초기화 실패: {ex}");
        }

    }

    /// <summary>
    /// 현재 씬에서 플레이어를 찾고 등록하는 메서드
    /// 플레이어가 있는 씬으로 이동할 때마다 호출해야함.
    /// </summary>
    public void FindPlayer()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedCalss)
    {
        selectedCharacterClass = selectedCalss;
    }

    public void AddCharacterSprite(CharacterClass character)
    {
        if (!(CharacterSpriteDicitonary.ContainsKey(character)))
        {
            CharacterSpriteDicitonary[character] = new SpriteData();
        }
    }

    /// <summary>
    /// 플레이어의 입력을 연결하는 메서드
    /// </summary>
    public void PlayerOnConnected()
    {
        Player.input.InputEvent();
    }

    /// <summary>
    /// 플레이어의 입력을 해제하는 메서드
    /// </summary>
    public void PlayerOffConnected()
    {
        Player.input.OutPutEvent();
    }

    /// <summary>
    /// 선택된 클래스 반환해줌
    /// </summary>
    /// <param name="selectedCalss"></param>
    /// <returns></returns>
    public CharacterClass GetSelectedClass()
    {
        return selectedCharacterClass;
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
        WeaponTop = await LoadAndSortSprites(Data.WeaponTopName);
        ClothTop = await LoadAndSortSprites(Data.ClothTopName);
        HairTop = await LoadAndSortSprites(Data.HairTopName);
        ClothBottom = await LoadAndSortSprites(Data.ClothBottomName);
        HairBottom = await LoadAndSortSprites(Data.HairBottomName);
        Face = await LoadAndSortSprites(Data.FaceName);
        Skin = await LoadAndSortSprites(Data.SkinName);
        WeaponBottom = await LoadAndSortSprites(Data.WeaponBottomName);
    }


    private async System.Threading.Tasks.Task<Dictionary<AnimationState, Sprite[]>> LoadAndSortSprites(string addressKey)
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
