using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerSpriteData PlayerSpriteData {  get; private set; }

    private Dictionary<CharacterClass, AnimatorOverrideController> animatorMap;    // 클래스마다 변경될 오버라이드 컨트롤러 가져야됨

    [SerializeField, ReadOnly] private CharacterClass selectedCharacterClass;
    public CharacterClass SelectedCharacterClass => selectedCharacterClass;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        PlayerSpriteData = Resources.Load<PlayerSpriteData>("Player/PlayerSpriteData/PlayerSpriteData");

        animatorMap = new Dictionary<CharacterClass, AnimatorOverrideController>();
    }

    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedCalss)
    {
        selectedCharacterClass = selectedCalss;
    }
}
