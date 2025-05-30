using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterClassDatas", menuName = "GameData/CharacterClassDatas")]
public class CharacterClassDatas : ScriptableObject
{
    public List<CharacterClassData> classDataList;
}

[System.Serializable]
public class CharacterClassData
{
    public CharacterClass characterClass;
    public string classNameKorean;
    [TextArea] public string description;
    public Sprite classIcon;
}
