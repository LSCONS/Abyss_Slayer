using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class BuffSlotPresenter : IPresenter
{
    private Player model;
    private UIBuffSlot view;
    private CompositeDisposable disposable = new();

    public BuffSlotPresenter(Player model, UIBuffSlot view)
    {
        this.model = model;
        this.view = view;
        //TODO: 버프 UI출력 교체 필요
        //view.SetIcon(GetBuffIcon()); // 버프 아이콘 설정
        //view.SetCoolTime(model.CurDuration.Value / model.MaxDuration.Value);
        //view.SetPresenter(this);

        //// 지속 시간에 따라 fillAmount 업데이트
        //model.CurDuration
        //    .CombineLatest(model.MaxDuration, (cur, max) => cur / max)
        //    .Subscribe(view.SetCoolTime)
        //    .AddTo(disposable);

        //// 지속 시간이 0 이하가 되면 UI만 끔 (SetActive(false)하기)
        //model.CurDuration
        //    .Where(cur => cur <= 0)
        //    .Subscribe(_ =>
        //    {
        //        view.gameObject.SetActive(false);
        //    })
        //    .AddTo(disposable);
    }

    private Sprite GetBuffIcon()
    {
        // 필요 시, 버프 발동한 스킬의 아이콘 반환
        foreach (var skill in model.equippedSkills.Values)
        {
            if (skill is BuffSkill)
                return skill.icon;
        }

        return null;
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

}
