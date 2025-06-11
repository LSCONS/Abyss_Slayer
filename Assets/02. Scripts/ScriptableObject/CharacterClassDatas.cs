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
    [field: SerializeField] public CharacterClass CharacterClass { get; private set; } = CharacterClass.Rogue;
    [field: SerializeField] public string ClassNameKorean { get; private set; } = "";
    [field: SerializeField, TextArea] public string Description { get; private set; } = "";
    [field: SerializeField] public CharacterData CharacterData { get; private set; } = null;
    [field: SerializeField] public CharacterSkillSet CharacterSkillSet { get; private set; } = null;
    [field: SerializeField] public Sprite IconClass { get; private set; } = null;
}
