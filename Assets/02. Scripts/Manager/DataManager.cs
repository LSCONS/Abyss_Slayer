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

public class DataManager
{
    #region Dicitonary데이터들
    //AnimationCurve값을 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<EAniamtionCurve, AnimationCurve> DictEnumToCurve { get; private set; } = new();

    //보스의 NetworkObject를 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<EBossStage, NetworkObject> DictEnumToBossObjcet { get; private set; } = new();

    //AudioClipData를 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<EAudioClip, AudioClipData> DictEnumToAudioData { get; private set; } = new();

    //Animator를 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<EAnimatorController, RuntimeAnimatorController> DictEnumToAnimatorData { get; private set; } = new();

    //CharacterClassData을 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<CharacterClass, CharacterClassData> DictClassToCharacterData { get; private set; } = new();

    //캐릭터의 각 행동에 따른 Sprite를 가지고 있는 딕셔너리
    public Dictionary<(int style, int color), Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleTopSprite { get; set; } = new();
    public Dictionary<(int style, int color), Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleBottomSprite { get; set; } = new();
    public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToFaceColorSprite { get; set; } = new();
    public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToSkinColorSprite { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToWeaponTop { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToWeaponBot { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToClothTop { get; set; } = new();
    public Dictionary<CharacterClass, Dictionary<AnimationState, Sprite[]>> DictClassToStateToClothBot { get; set; } = new();

    //버프 이펙트 Sprite를 enum 값으로 찾기 위한 딕셔너리
    public Dictionary<EBuffType, Sprite> DictBuffToSprite { get; set; } = new();

    public Dictionary<EType, Type> DictEnumToType { get; set; } = new()
    {
        { EType.BossHitEffect, typeof(BossHitEffect) },
    };
    #endregion

    #region Prefab데이터들
    public PoolManager PoolManagerPrefab { get; private set; }
    public InitSupporter InitSupporterPrefab { get; private set; }
    public NetworkData PlayerNetworkDataPrefab { get; private set; }
    public Player PlayerPrefab { get; private set; }
    public GameObject DashEffectPrefab { get; private set; }
    public FadeController ShieldPrefab { get; private set; }
    public NetworkObjectFollowServer CrossHairPrefab { get; private set; }
    public UITeamStatusSlot PlayerStatusPrefab { get; private set; }
    public Fireworks FireworksPrefab { get; private set; }
    public List<BasePoolable> ListBasePoolablePrefab { get; private set; } = new();
    #endregion

    private int[] HairColorVariants { get; set; } = new int[] { 1, 2, 4, 5, 6, 10 };  // 클래스별 머리색 c1,c2...
    public int MaxHairFKey, MaxHairMKey, MaxSkinKey, MaxFaceKey;

    public async Task Init()
    {
        await DataLoadAnimationCurveData();
        await DataLoadBossPrefabData();
        await DataLoadAudioClipData();
        await DataLoadAnimationSpriteData();
        await DataLoadEachData();
        await DataLoadCharacterToSpriteData();
        await DataLoadPoolObjectData();
        await DataLoadAnimatorControllerData();
        await DataLoadBuffSpriteData();
        await LoadCharacterClassData();

        return;
    }


    public async Task DataLoadBuffSpriteData()
    {
#if AllMethodDebug
        Debug.Log("DataLoadBuffSpriteData");
#endif
        var data = Addressables.LoadAssetAsync<BuffSpriteDataGather>("BuffSpriteDatas");
        await data.Task;
        foreach (BuffSpriteData buffData in data.Result.ListBuffImageData)
        {
            if (buffData.BuffSprite == null) continue;
            DictBuffToSprite[buffData.EBuffType] = buffData.BuffSprite;
        }
        return;
    }


    /// <summary>
    /// PoolObject가 가질 Data들을 저장할 메서드
    /// </summary>
    /// <returns></returns>
    public async Task DataLoadPoolObjectData()
    {
#if AllMethodDebug
        Debug.Log("DataLoadPoolObjectData");
#endif
        HashSet<Type> types = new();
        var poolablePrefabs = Addressables.LoadAssetsAsync<GameObject>
        (
            "PoolablePrefab",
            prefab =>
            {
                BasePoolable poolable = prefab.GetComponent<BasePoolable>();
                if (poolable == null) return;
                if (types.Contains(poolable.GetType())) return;
                ListBasePoolablePrefab.Add(poolable);
                types.Add(poolable.GetType());
            }

        );
        await poolablePrefabs.Task;
        return;
    }


    /// <summary>
    /// 캐릭터 직업별 옷, 무기의 Sprite만 저장하는 메서드
    /// </summary>
    /// <returns></returns>
    private async Task DataLoadCharacterToSpriteData()
    {
#if AllMethodDebug
        Debug.Log("DataLoadCharacterToSpriteData");
#endif
        PlayerSpriteData data = new PlayerSpriteData();
        foreach (CharacterClass character in Enum.GetValues(typeof(CharacterClass)))
        {
            if (character == CharacterClass.Count) continue;
            data.SetSpriteName(character);
            DictClassToStateToClothBot[character] = await LoadAndSortSprites(data.ClothBottomName);
            DictClassToStateToClothTop[character] = await LoadAndSortSprites(data.ClothTopName);
            DictClassToStateToWeaponBot[character] = await LoadAndSortSprites(data.WeaponBottomName);
            DictClassToStateToWeaponTop[character] = await LoadAndSortSprites(data.WeaponTopName);
        }
    }


    /// <summary>
    /// 개별 오브젝트를 로드하는 메서드
    /// </summary>
    /// <returns></returns>
    private async Task DataLoadEachData()
    {
#if AllMethodDebug
        Debug.Log("DataLoadEachData");
#endif
        var init                = await Util.GetAddressableData<InitSupporter>              ("InitSupporter"    );
        var pool                = await Util.GetAddressableData<PoolManager>                ("PoolManager"      );
        var player              = await Util.GetAddressableData<Player>                     ("PlayerPrefab"     );
        var crossHair           = await Util.GetAddressableData<NetworkObjectFollowServer>  ("CrossHairPrefab"  );
        var networkData         = await Util.GetAddressableData<NetworkData>                ("NetworkData"      );
        var dashEffectPrefab    = await Util.GetAddressableData<GameObject>                 ("DashEffectPrefab" );
        var shieldPrefab        = await Util.GetAddressableData<FadeController>             ("ShieldPrefab"     );
        var teamStatusSlot      = await Util.GetAddressableData<UITeamStatusSlot>           ("TeamStatusSlot"   );
        var fireworksPrefab     = await Util.GetAddressableData<Fireworks>                  ("FireworksPrefab"  );
        return;
    }


    /// <summary>
    /// 헤어, 스킨, 얼굴의 sprite들을 어드레서블 string값으로 확인하고 잘라서 저장하는 메서드
    /// </summary>
    /// <returns></returns>
    private async Task DataLoadAnimationSpriteData()
    {
        MaxHairFKey = 9;
        MaxHairMKey = 14;
        MaxFaceKey = 7;
        MaxSkinKey = 6;


        // 6. face_c1 ~ face_c7
        for (int i = 1; i <= MaxFaceKey; i++)
        {
            DictIntToDictStateToFaceColorSprite[i] = await LoadAndSortSprites($"face_c{i}");
        }

        // skin_c1 ~ skin_c6
        for (int i = 1; i <= MaxSkinKey; i++)
        {
            DictIntToDictStateToSkinColorSprite[i] = await LoadAndSortSprites($"skin_c{i}");
        }

        // 머리색 클래스마다 다르기 떄문에 다 로드해줘야됨
        foreach (var colorIndex in HairColorVariants)
        {
            for (int i = 1; i <= MaxHairFKey; i++)
            {
                string keyTop = $"f{i}_c{colorIndex}_top";
                string keyBot = $"f{i}_c{colorIndex}_bot";

                DictIntToDictStateToHairStyleTopSprite[(i, colorIndex)] = await LoadAndSortSprites(keyTop);
                DictIntToDictStateToHairStyleBottomSprite[(i, colorIndex)] = await LoadAndSortSprites(keyBot);
            }

            // 8. m1~m14 헤어
            for (int i = 1; i <= MaxHairMKey; i++)
            {
                string keyTop = $"m{i}_c{colorIndex}_top";
                string keyBot = $"m{i}_c{colorIndex}_bot";

                DictIntToDictStateToHairStyleTopSprite[(i + MaxHairFKey, colorIndex)] = await LoadAndSortSprites(keyTop);
                DictIntToDictStateToHairStyleBottomSprite[(i + MaxHairFKey, colorIndex)] = await LoadAndSortSprites(keyBot);
            }
        }
    }


    /// <summary>
    /// 넣은 어드레서블 키 값에 해당하는 Sprite를 잘라서 반환해주는 메서드
    /// </summary>
    /// <param name="addressKey"></param>
    /// <returns></returns>
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
        foreach (AnimationCurveData animationCurveStruct in GatherData.ListAnimationCurveData)
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
            DictEnumToBossObjcet[bossPrefabStruct.BossStage] = bossPrefabStruct.BossObject;
        }
        return;
    }


    /// <summary>
    /// 음악 클립 데이터를 로드하는 메서드
    /// </summary>
    private async Task DataLoadAudioClipData()
    {
        var AudioClipData = Addressables.LoadAssetsAsync<AudioClipDataGather>("AuidoClipDatas", null);
        await AudioClipData.Task;
        AudioClipDataGather[] gatherData = AudioClipData.Result.ToArray();
        foreach (AudioClipDataGather gather in gatherData)
        {
            foreach (AudioClipEnumData data in gather.ListAudioClipEnumData)
            {
                DictEnumToAudioData[data.EnumClip] = data.AudioClipData;
            }
        }
        return;
    }


    /// <summary>
    /// 애니메이터 데이터를 로드하는 메서드
    /// </summary>
    private async Task DataLoadAnimatorControllerData()
    {
        var AnimatorControllerData = Addressables.LoadAssetsAsync<AnimatorControllerDataGather>("AnimatorController", null);
        await AnimatorControllerData.Task;
        AnimatorControllerDataGather[] gatherData = AnimatorControllerData.Result.ToArray();
        foreach (AnimatorControllerDataGather gather in gatherData)
        {
            foreach (AnimatorControllerData data in gather.ListAnimatorController)
            {
                if (data.AnimatorController == null) continue;
                DictEnumToAnimatorData[data.EAnimatorContoller] = data.AnimatorController;
            }
        }
        return;
    }


    /// <summary>
    /// 딕셔너리를 복사해서 반환해주는 메서드
    /// </summary>
    /// 
    /// <param name="data"></param>
    /// <returns></returns>
    public Dictionary<AnimationState, Sprite[]> InstantiateDictionary(Dictionary<AnimationState, Sprite[]> data)
    {
        Dictionary<AnimationState, Sprite[]> result = new();
        foreach (AnimationState state in data.Keys)
        {
            Sprite[] sprites = new Sprite[data[state].Length];
            for (int i = 0; i < data[state].Length; i++)
            {
                sprites[i] = data[state][i];
            }
            result[state] = sprites;
        }
        return result;
    }

    /// <summary>
    /// 캐릭터 클래스 데이터 로드
    /// </summary>
    /// <returns></returns>
    private async Task LoadCharacterClassData()
    {
#if AllMethodDebug
        Debug.Log("LoadCharacterClassData");
#endif
        var handle = Addressables.LoadAssetAsync<CharacterClassDatas>("CharacterClassDatas");
        await handle.Task;

        var gather = handle.Result;
        DictClassToCharacterData = new();

        foreach (var data in gather.classDataList)
        {
            if (!DictClassToCharacterData.ContainsKey(data.CharacterClass))
                DictClassToCharacterData[data.CharacterClass] = data;
        }
    }

}

    public enum EType
    {
        None = 0,
        BossHitEffect,
    }
