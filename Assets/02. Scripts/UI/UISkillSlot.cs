using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
public class UISkillSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;


    private CompositeDisposable compositeDisposable = new();
    public void Bind(SkillData skillData)
    {
        iconImage.sprite = skillData.icon;
        coolTimeOverlay.fillAmount = skillData.CurCoolTime.Value / skillData.MaxCoolTime.Value;
        coolTimeText.text = skillData.CurCoolTime.Value.ToString("F0");

        skillData.CurCoolTime.CombineLatest(skillData.MaxCoolTime, (cur, max) => cur / max)
        .Subscribe(ratio => coolTimeOverlay.fillAmount = ratio)
        .AddTo(compositeDisposable);
    }

    private void OnDestroy()
    {
        compositeDisposable.Clear();
    }

}
