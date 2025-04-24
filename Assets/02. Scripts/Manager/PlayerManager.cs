using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerAnimationData PlayerAnimationData {  get; private set; }

    private Dictionary<CharacterClass, AnimatorOverrideController> animatorMap;    // 클래스마다 변경될 오버라이드 컨트롤러 가져야됨

    [SerializeField, ReadOnly] private CharacterClass selectedCharacterClass;
    public CharacterClass SelectedCharacterClass => selectedCharacterClass;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        PlayerAnimationData = Resources.Load<PlayerAnimationData>("Player/PlayerAnimationData/PlayerAnimationData");
        PlayerAnimationData.Init();

        animatorMap = new Dictionary<CharacterClass, AnimatorOverrideController>();
    }

    public void SettingPlayerAnimator(CharacterClass selectedCalss, Animator targetAnimator)
    {
        if(!animatorMap.TryGetValue(selectedCalss, out var controller))
        {
            controller = Resources.Load<AnimatorOverrideController>($"Player/PlayerAnimationContorller/{selectedCalss}");
            if (controller != null)
            {
                animatorMap[selectedCalss] = controller;
            }
            else return;
        }
        targetAnimator.runtimeAnimatorController = controller;
    }

    // 클래스 세팅해주는 메서드
    public void SetSelectedClass(CharacterClass selectedCalss)
    {
        selectedCharacterClass = selectedCalss;
    }
}
