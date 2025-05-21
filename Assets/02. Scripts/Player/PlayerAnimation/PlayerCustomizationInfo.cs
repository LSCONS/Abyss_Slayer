using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerCustomizationInfo
{
    public int skinId;
    public int faceId;
    public HairStyleInfo hairInfo;

    public PlayerCustomizationInfo(int skinId, int faceId, (int, int) hairId)
    {
        this.skinId = skinId;
        this.faceId = faceId;
        this.hairInfo = new HairStyleInfo(hairId.Item1, hairId.Item2);
    }

    public (int, int) hairStyleTuple => hairInfo.ToTuple();
}

[Serializable]
public class HairStyleInfo
{
    public int styleId;
    public int colorIndex;

    public HairStyleInfo(int styleId, int colorIndex)
    {
        this.styleId = styleId;
        this.colorIndex = colorIndex;
    }

    public (int, int) ToTuple() => (styleId, colorIndex);
}
