using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerSpriteData PlayerSpriteData {  get; private set; }

    public CharacterClass selectedCharacterClass;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        PlayerSpriteData = Resources.Load<PlayerSpriteData>("Player/PlayerSpriteData/PlayerSpriteData");
    }

    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedCalss)
    {
        selectedCharacterClass = selectedCalss;
    }

    /// <summary>
    /// 선택된 클래스 반환해줌
    /// </summary>
    /// <param name="selectedCalss"></param>
    /// <returns></returns>
    public CharacterClass GetSelectedClass()
    {
        return selectedCharacterClass;
    }
}
