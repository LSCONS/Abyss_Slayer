using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClassSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private Button button;
    [SerializeField] private Image icon;

    private CharacterClass characterClass;
    public CharacterClass CharacterClass => characterClass;


    private Subject<CharacterClass> onClickSubject = new Subject<CharacterClass>();
    public IObservable<CharacterClass> OnClickAsObservable => onClickSubject;

    public void SetIcon(CharacterClass cls)
    {
        if (DataManager.Instance.DictClassToImage.TryGetValue(cls, out var classImage))
        {
            icon.sprite = classImage;
        }
        else return;
    }

    public void SetData(CharacterClass cls)
    {
        characterClass = cls;

        if (DataManager.Instance.DictCharacterClassData.TryGetValue(cls, out var data))
        {
            classText.text = data.classNameKorean;
            icon.sprite = data.classIcon;
        }

        if (button == null)
            button = GetComponent<Button>();

        // 버튼 클릭하면 characterClass 값 전달
        button.OnClickAsObservable()
              .Subscribe(_ => 
              {
                  SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
                  onClickSubject.OnNext(characterClass);
              })
              .AddTo(this);
    }
}



// 클래스 한국어 변환 / 설명 넣음 
// TODO: 나중에 데이터 파싱필요
public static class CharacterClassExtensions
{
    public static string ToKoreanString(this CharacterClass cls)
    {
        return cls switch
        {
            CharacterClass.Rogue => "도적",
            CharacterClass.Healer => "힐러",
            CharacterClass.Mage => "마법사",
            CharacterClass.MagicalBlader => "마검사",
            CharacterClass.Tanker => "탱커",
            _ => cls.ToString()
        };
    }

    public static string GetDescription(this CharacterClass cls)
    {
        return cls switch
        {
            CharacterClass.Rogue => "원거리에서 빠르게 공격하는 도적입니다.",
            CharacterClass.Healer => "팀을 치유하고 보호하는 힐러입니다.",
            CharacterClass.Mage => "강력한 마법을 사용하는 마법사입니다.",
            CharacterClass.MagicalBlader => "검과 마법을 함께 사용하는 마검사입니다.",
            CharacterClass.Tanker => "공격을 버텨내는 탱커입니다.",
            _ => "알 수 없음"
        };
    }
}
