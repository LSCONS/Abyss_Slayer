using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// 클래스별 허용 머리 색깔들 인덱스 추출해야됨
/// 미리 정해놓은 색으로
/// </summary>
public static class HairColorConfig
{
    public static readonly Dictionary<CharacterClass, int> HairColorIndexByClass = new()
    {
        { CharacterClass.Mage,           6 },
        { CharacterClass.Tanker,         10  },
        { CharacterClass.MagicalBlader,  4 },
        { CharacterClass.Healer,         2 },
        { CharacterClass.Rogue,          5 },
    };
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<EAniamtionCurve, AnimationCurve> DictEnumToCurve { get; private set; } = new();
    public Dictionary<EBossStage, NetworkObject> DictEnumToNetObjcet { get; private set; } = new();
    public Dictionary<EAudioClip, AudioClipData> DictEnumToAudioData { get; private set; } = new();
    public Dictionary<CharacterClass, CharacterSkillSet> DictClassToSkillSet { get; private set; } = new();
    public Dictionary<CharacterClass, PlayerData> DictClassToPlayerData { get; private set; } = new();
    public PoolManager PoolManager { get; private set; }
    public InitSupporter InitSupporter { get; private set; }
    public Player Player { get; private set; }
    //첫 키의 int는 스타일, 두번 째 키의 int는 Color
    public Dictionary<(int, int), Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleTopSprite { get; set; } = new();
    public Dictionary<(int, int), Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleBottomSprite { get; set; } = new();
    public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToFaceColorSprite { get; set; } = new();
    public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToSkinColorSprite { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToWeaponTop { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToWeaponbot { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToClothTop { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToClothbot { get; set; } = new();
    private int[] HairColorVariants { get; set; } = new int[] { 1, 2, 4, 5, 6, 10 };  // 클래스별 머리색 c1,c2...


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public async Task Init()
    {
        await DataLoadAnimationCurveData();
        await DataLoadBossPrefabData();
        await DataLoadAudioClipData();
        await DataLoadAnimationSpriteData();
        await DataLoadInitInitSupporterData();
        await LoadSkillSetData();
        await LoadPlayerData();
        await DataLoadCharacterToSpriteData();


        return;
    }


    private async Task DataLoadCharacterToSpriteData()
    {
        PlayerSpriteData data = new PlayerSpriteData();
        foreach (CharacterClass character in Enum.GetValues(typeof(CharacterClass)))
        {
            if (character == CharacterClass.Count) continue;
            data.SetSpriteName(character);
            DictClassToStateToClothbot[character] = await LoadAndSortSprites(data.ClothBottomName);
            DictClassToStateToClothTop[character] = await LoadAndSortSprites(data.ClothTopName);
            DictClassToStateToWeaponbot[character] = await LoadAndSortSprites(data.WeaponBottomName);
            DictClassToStateToWeaponTop[character] = await LoadAndSortSprites(data.WeaponTopName);
        }
    }


    private async Task DataLoadInitInitSupporterData()
    {
#if AllMethodDebug
        Debug.Log("DataLoadInitInitSupporterData");
#endif
        var init = Addressables.LoadAssetAsync<GameObject>("InitSupporter");         // 우선 스프라이트 시트를 로드함 Sprite[]로 로드해서 스프라이트를 가져옴
        await init.Task;
        InitSupporter = init.Result.GetComponent<InitSupporter>();
        if (InitSupporter == null) { Debug.Log("Error InitSupporter is null"); }


        var pool = Addressables.LoadAssetAsync<GameObject>("PoolManager");         // 우선 스프라이트 시트를 로드함 Sprite[]로 로드해서 스프라이트를 가져옴
        await pool.Task;
        PoolManager = pool.Result.GetComponent<PoolManager>();
        if (PoolManager == null) { Debug.Log("Error PoolManager is null"); }

        var player = Addressables.LoadAssetAsync<GameObject>("PlayerPrefab");
        await player.Task;
        Player = player.Result.GetComponent<Player>();
        if (Player == null) { Debug.Log("Error Player is null"); }

        return;
    }

    private async Task LoadSkillSetData()
    {
        try
        {
            var data = Addressables.LoadAssetsAsync<CharacterSkillSet>("CharacterSkillSet", null);
            await data.Task;
            foreach (CharacterSkillSet character in data.Result)
            {
                DictClassToSkillSet[character.Class] = character;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SkillSetData초기화 실패: {ex}");
        }
        return;
    }


    private async Task LoadPlayerData()
    {
        try
        {
            var data = Addressables.LoadAssetsAsync<PlayerData>("CharacterData", null);
            await data.Task;
            foreach (PlayerData character in data.Result)
            {
                DictClassToPlayerData[character.PlayerStatusData.Class] = character;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"PlayerData초기화 실패: {ex}");
        }
        return;
    }

    private async Task DataLoadAnimationSpriteData()
    {
        // 6. face_c1 ~ face_c7
        for (int i = 1; i <= 7; i++)
        {
            DictIntToDictStateToFaceColorSprite[i] = await LoadAndSortSprites($"face_c{i}");
        }

        // skin_c1 ~ skin_c6
        for (int i = 1; i <= 6; i++)
        {
            DictIntToDictStateToSkinColorSprite[i] = await LoadAndSortSprites($"skin_c{i}");
        }

        // 머리색 클래스마다 다르기 떄문에 다 로드해줘야됨
        foreach (var colorIndex in HairColorVariants)
        {
            for (int i = 1; i <= 9; i++)
            {
                string keyTop = $"f{i}_c{colorIndex}_top";
                string keyBot = $"f{i}_c{colorIndex}_bot";

                int dictKey = CreateHairKey($"f{i}", colorIndex);
                DictIntToDictStateToHairStyleTopSprite[(i, colorIndex)] = await LoadAndSortSprites(keyTop);
                DictIntToDictStateToHairStyleBottomSprite[(i, colorIndex)] = await LoadAndSortSprites(keyBot);
            }

        // 8. m1~m14 헤어
            for (int i = 1; i <= 14; i++)
            {
                string keyTop = $"m{i}_c{colorIndex}_top";
                string keyBot = $"m{i}_c{colorIndex}_bot";

                int dictKey = CreateHairKey($"m{i}", colorIndex);
                DictIntToDictStateToHairStyleTopSprite[(i+9, colorIndex)] = await LoadAndSortSprites(keyTop);
                DictIntToDictStateToHairStyleBottomSprite[(i+9, colorIndex)] = await LoadAndSortSprites(keyBot);
            }
        }
    }
    /// <summary>
    /// 딕셔너리 키 생성을 위한 메서드 
    /// m4이고 c5 이면 1405로 키 생성해줌
    /// f4이고 c5 이면 4405로 키 생성
    /// </summary>
    /// <param name="styleId"></param>
    /// <param name="colorIndex"></param>
    /// <returns></returns>
    private int CreateHairKey(string styleId, int colorIndex)
    {
        int baseOffset = styleId.StartsWith("m") ? 1000 : 4000;
        int styleNum = int.Parse(styleId.Substring(1));
        return baseOffset + styleNum * 100 + colorIndex;
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


    /// <summary>
    /// 애니메이션커브 데이터를 로드하는 메서드
    /// </summary>
    private async Task DataLoadAnimationCurveData()
    {
        var animationCurveData = Addressables.LoadAssetAsync<AnimationCurveDataGather>("AnimationCurveDatas");
        await animationCurveData.Task;
        AnimationCurveDataGather GatherData = animationCurveData.Result;
        foreach(AnimationCurveData animationCurveStruct in GatherData.ListAnimationCurveData)
    {
            DictEnumToCurve[animationCurveStruct.EAnimationCurve] = animationCurveStruct.AnimationCurve;
        }
        return;
    }


    /// <summary>
    /// 보스 프리팹 데이터를 로드하는 메서드
    /// </summary>
    private async Task DataLoadBossPrefabData()
    {
        var bossPrefabData = Addressables.LoadAssetAsync<BossPrefabDataGather>("BossPrefabDatas");
        await bossPrefabData.Task;
        BossPrefabDataGather data = bossPrefabData.Result;
        foreach (BossPrefabData bossPrefabStruct in data.ListBossPrefabData)
    {
            DictEnumToNetObjcet[bossPrefabStruct.BossStage] = bossPrefabStruct.BossObject;
        }
        return;
    }


    /// <summary>
    /// 음악 클립 데이터를 로드하는 메서드
    /// </summary>
    private async Task DataLoadAudioClipData()
    {
        var AudioClipData = Addressables.LoadAssetAsync<AudioClipDataGather>("AuidoClipDatas");
        await AudioClipData.Task;
        AudioClipDataGather gatherData = AudioClipData.Result;
        foreach (AudioClipEnumData data in gatherData.ListAudioClipEnumData)
    {
            DictEnumToAudioData[data.EnumClip] = data.AudioClipData;
        }
        return;
    }
}
