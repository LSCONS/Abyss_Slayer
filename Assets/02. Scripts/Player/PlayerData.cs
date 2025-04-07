using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Characters/Player")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField]public PlayerGroundData PlayerGroundData { get; private set; }
    [field: SerializeField]public PlayerAirData PlayerAirData { get; private set; }
}


[Serializable]
public class PlayerAirData
{
    [field: Header("JumpData")]
    [field: SerializeField] public float WalkSpeedModifier { get; private set; } = 0.225f;
    [field: SerializeField] public float JumpForce { get; private set; } = 10f;

    [field: Header("DashData")]
    [field: SerializeField] public float DashForce { get; private set; } = 5f;
}


[Serializable]
public class PlayerGroundData
{
    [field: Header("IdleData")]
    [field: SerializeField] public float BaseSpeed { get; private set; } = 10f;
    [field: SerializeField] public float BaseRotationDamping { get; private set; } = 1f;

    [field: Header("WalkData")]
    [field: SerializeField] public float WalkSpeedModifier { get; private set; } = 0.225f;

    [field: Header("RunData")]
    [field: SerializeField] public float RunSpeedModifier { get; private set; } = 1f;
}


[Serializable]
public class PlayerAbnormal
{

}
