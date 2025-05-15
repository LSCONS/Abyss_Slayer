using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AnimationCurve", menuName = "Data/AnimatinCurve")]
public class AnimationCurveData : ScriptableObject
{
    [field: SerializeField] public List<AnimationCurveStruct> AnimationCurveDataList;
}


[Serializable]
public struct AnimationCurveStruct
{
    [field: SerializeField] public EAniamtionCurve EAnimationCurve { get; private set; }
    [field: SerializeField] public AnimationCurve AnimationCurve { get; private set; }
}


public enum EAniamtionCurve
{
    None = 0,
    Boss0HormingCurve,
    Boss0SpeedCurve,
}
