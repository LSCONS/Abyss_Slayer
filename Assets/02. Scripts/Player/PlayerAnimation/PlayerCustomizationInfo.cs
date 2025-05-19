using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerCustomizationInfo
{
    public int skinId;
    public int faceId;
    public int hairId;

    public PlayerCustomizationInfo(int skinId, int faceId, int hairId)
    {
        this.skinId = skinId;
        this.faceId = faceId;
        this.hairId = hairId;
    }
}
