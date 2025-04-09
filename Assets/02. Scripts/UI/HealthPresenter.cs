using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HealthPresenter : IPresenter
{
    private readonly IHasHealth model;
    private readonly UIHealthBar view;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public HealthPresenter(IHasHealth model, UIHealthBar view)
    {
        this.model = model;
        this.view = view;

        // 모델의 Hp와 MaxHp를 구독하고, 뷰의 SetHp 메서드를 호출하여 값을 설정
        Observable.CombineLatest(model.Hp, model.MaxHp,
        (hp, maxHp) => Mathf.Clamp01((float)hp / maxHp))
        .Subscribe(value => view.SetHp(value))
        .AddTo(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
