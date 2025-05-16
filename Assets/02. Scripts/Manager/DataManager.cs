using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class DataManager : Singleton<DataManager>
{
    //public Dictionary<EAniamtionCurve, AnimationCurve> DictEnumToCurve { get; private set; } = new();
    //public Dictionary<EBossStage, NetworkObject> DictEnumToNetObjcet { get; private set; } = new();
    [field: SerializeField] public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleTopSprite { get; set; } = new();
    [field: SerializeField] public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToHairStyleBottomSprite { get; set; } = new();
    [field: SerializeField] public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToFaceColorSprite { get; set; } = new();
    [field: SerializeField] public Dictionary<int, Dictionary<AnimationState, Sprite[]>> DictIntToDictStateToSkinColorSprite { get; set; } = new();


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        DataLoadAnimationCurveData();
        DataLoadBossPrefabData();
        DataLoadAudioClipData();
        DataLoadAnimationSpriteData();
    }


    private async void DataLoadAnimationSpriteData()
    {
        // 6. face_c1 ~ face_c7
        for (int i = 1; i <= 7; i++)
        {
            DictIntToDictStateToFaceColorSprite[i] = await LoadAndSortSprites($"face_c{i}");
        }

        // 7. skin_c1 ~ skin_c6
        for (int i = 1; i <= 6; i++)
        {
            DictIntToDictStateToSkinColorSprite[i] = await LoadAndSortSprites($"skin_c{i}");
        }

        // 8. f1~f9 / m1~m14 헤어 top
        for (int i = 1; i <= 9; i++)
        {
            DictIntToDictStateToHairStyleTopSprite[i] = await LoadAndSortSprites($"f{i}_c1_top");
            DictIntToDictStateToHairStyleBottomSprite[i] = await LoadAndSortSprites($"f{i}_c1_top");
        }
        for (int i = 1; i <= 14; i++)
        {
            DictIntToDictStateToHairStyleTopSprite[i] = await LoadAndSortSprites($"m{i}_c1_top");
            DictIntToDictStateToHairStyleBottomSprite[i] = await LoadAndSortSprites($"m{i}_c1_top");
        }
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
    private async void DataLoadAnimationCurveData()
    {
        //var animationCurveData = Addressables.LoadAssetAsync<AnimationCurveData>("AnimationCurveData");
        //await animationCurveData.Task;
        //AnimationCurveData data = animationCurveData.Result;
        //foreach(AnimationCurveStruct animationCurveStruct in data.AnimationCurveDataList)
        //{
        //    DictEnumToCurve[animationCurveStruct.EAnimationCurve] = animationCurveStruct.AnimationCurve;
        //}
    }


    /// <summary>
    /// 보스 프리팹 데이터를 로드하는 메서드
    /// </summary>
    private async void DataLoadBossPrefabData()
    {
        //var bossPrefabData = Addressables.LoadAssetAsync<BossPrefabData>("BossPrefabData");
        //await bossPrefabData.Task;
        //BossPrefabData data = bossPrefabData.Result;
        //foreach (BossPrefabStruct bossPrefabStruct in data.ListBossPrefabStruct)
        //{
        //    DictEnumToNetObjcet[bossPrefabStruct.BossStage] = bossPrefabStruct.BossObject;
        //}
    }


    /// <summary>
    /// 음악 클립 데이터를 로드하는 메서드
    /// </summary>
    private async void DataLoadAudioClipData()
    {
        //var AudioClipData = Addressables.LoadAssetAsync<AnimationCurveData>("");
        //await AudioClipData.Task;
        //AnimationCurveData data = AudioClipData.Result;
        //foreach (AnimationCurveStruct animationCurveStruct in data.AnimationCurveDataList)
        //{
        //    DictEnumToCurve[animationCurveStruct.EAnimationCurve] = animationCurveStruct.AnimationCurve;
        //}
    }
}
