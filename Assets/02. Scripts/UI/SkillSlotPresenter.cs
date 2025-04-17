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
        view.SetIcon(model.icon);
        view.SetCoolTime(model.CurCoolTime.Value);
        view.SetPresenter(this);

        model.CurCoolTime
            .CombineLatest(model.MaxCoolTime, (cur, max) => max <= 0 ? 0f : cur / max)  // 0 나누면 non됨 0이하로 나누면 0으로 처리
            .Subscribe(view.SetCoolTime)
            .AddTo(disposable);
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

}
