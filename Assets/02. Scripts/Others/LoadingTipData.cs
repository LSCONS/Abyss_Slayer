using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LoadingTipData", menuName = "GameData/LoadingTipData")]
public class LoadingTipData : ScriptableObject
{
    [TextArea(1, 5)]
    public List<string> tips = new();
}
