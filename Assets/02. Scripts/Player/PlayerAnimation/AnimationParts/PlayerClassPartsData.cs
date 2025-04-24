using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ClassParts
{
    public CharacterClass characterClass;
    public string clothKey;
    public string weaponKey;
}

[CreateAssetMenu(fileName = "PlayerClassPartsData", menuName = "Player/ClassPartsData")]
public class PlayerClassPartsData : ScriptableObject
{
    public List<ClassParts> classPresets;
}
