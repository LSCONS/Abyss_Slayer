using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AnimationCurve", menuName = "Data/AnimatinCurve")]
public class AnimationCurveDataGather : ScriptableObject
{
    [field: SerializeField] public List<AnimationCurveData> ListAnimationCurveData;
}


[Serializable]
public class AnimationCurveData
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
