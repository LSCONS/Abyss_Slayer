using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HelpDatas
{
    [Tooltip("타이틀 예시) 방 만들기, 설명")]
    public string title;

    [TextArea] public string description;
    public Sprite image;
}

[CreateAssetMenu(fileName = "HelpRootData", menuName = "GameData/HelpRoot")]
public class HelpRootData : ScriptableObject
{
    [Header("도움말 전체 목록 (최상위 카테고리만 리스트로)")]
    public List<HelpData> rootHelpList;
}

[System.Serializable]
public class HelpData
{
    public HelpDatas helpDatas;

    [Tooltip("true면 상위 카테고리임")]
    public bool isCategory = false;

    [Header("하위 설명 항목들")]
    public List<HelpData> children;
}
