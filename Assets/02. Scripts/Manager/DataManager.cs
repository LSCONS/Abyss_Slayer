using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<EAniamtionCurve, AnimationCurve> DictEnumToCurve { get; private set; } = new();
    public Dictionary<EBossStage, NetworkObject> DictEnumToNetObjcet { get; private set; } = new();


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DataLoadAnimationCurveData();
        DataLoadBossPrefabData();
        DataLoadAudioClipData();
    }


    /// <summary>
    /// 애니메이션커브 데이터를 로드하는 메서드
    /// </summary>
    private async void DataLoadAnimationCurveData()
    {
        var animationCurveData = Addressables.LoadAssetAsync<AnimationCurveData>("AnimationCurveData");
        await animationCurveData.Task;
        AnimationCurveData data = animationCurveData.Result;
        foreach(AnimationCurveStruct animationCurveStruct in data.AnimationCurveDataList)
        {
            DictEnumToCurve[animationCurveStruct.EAnimationCurve] = animationCurveStruct.AnimationCurve;
        }
    }


    /// <summary>
    /// 보스 프리팹 데이터를 로드하는 메서드
    /// </summary>
    private async void DataLoadBossPrefabData()
    {
        var bossPrefabData = Addressables.LoadAssetAsync<BossPrefabData>("BossPrefabData");
        await bossPrefabData.Task;
        BossPrefabData data = bossPrefabData.Result;
        foreach (BossPrefabStruct bossPrefabStruct in data.ListBossPrefabStruct)
        {
            DictEnumToNetObjcet[bossPrefabStruct.BossStage] = bossPrefabStruct.BossObject;
        }
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
