using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntroCut
{
    public Sprite backgroundImage;
    [TextArea(2, 5)] public string line;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "IntroData", menuName = "GameData/Intro Data", order = 0)]
public class IntroData : ScriptableObject
{
    public List<IntroCut> cuts;
}
