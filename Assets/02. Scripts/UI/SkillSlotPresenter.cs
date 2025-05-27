using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class SkillSlotPresenter : IPresenter
{
    private Skill model;
    private UISkillSlot view;
    private CompositeDisposable disposable = new();

    public SkillSlotPresenter(Skill model, UISkillSlot view)
    {
        this.model = model;
        this.view = view;

        view.SetSkillData(model);
        view.SetIcon(model.SkillIcon);
        SetHoldIcon();
        view.SetCoolTime(model.CurCoolTime.Value, model.MaxCoolTime.Value);
        view.SetPresenter(this);
        view.SetKeyText(model.slotKey);
        model.CurCoolTime
            .Subscribe(cur => view.SetCoolTime(cur, model.MaxCoolTime.Value))
            .AddTo(disposable);
    }

    public void SetHoldIcon()
    {
        if (model.SkillCategory == SkillCategory.Hold)
        {
            view.SetHoldIcon(true);
        }
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

}
