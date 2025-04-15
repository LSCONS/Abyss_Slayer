using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class SkillSlotPresenter : IPresenter
{
    private SkillData model;
    private UISkillSlot view;
    private CompositeDisposable disposable = new();

    public SkillSlotPresenter(SkillData model, UISkillSlot view)
    {
        this.model = model;
        this.view = view;

        view.SetIcon(model.icon);
        view.SetCoolTime(model.CurCoolTime.Value);
        view.SetPresenter(this);

        model.CurCoolTime.CombineLatest(model.MaxCoolTime, (cur, max) => cur / max)
            .Subscribe(view.SetCoolTime)
            .AddTo(disposable);
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

}
