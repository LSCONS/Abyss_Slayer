using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class BuffSlotPresenter : IPresenter
{
    private BuffSkill model;
    private UIBuffSlot view;
    private CompositeDisposable disposable = new();

    public BuffSlotPresenter(BuffSkill model, UIBuffSlot view)
    {
        this.model = model;
        this.view = view;
        //TODO: 버프 UI출력 교체 필요
        view.SetBuffInfo(model.Info.Name, model.Info.Description);    // 버프 인포 설정
        view.SetIcon(GetBuffIcon()); // 버프 아이콘 설정
        view.SetCoolTime(model.CurBuffDuration.Value, model.MaxBuffDuration.Value);
        view.SetPresenter(this);

        // 지속 시간에 따라 fillAmount 업데이트
        model.CurBuffDuration
           .Subscribe(cur =>
           {
               view.SetCoolTime(cur, model.MaxBuffDuration.Value);
           })
           .AddTo(disposable);

        // 지속 시간이 0 이하가 되면 UI만 끔 (SetActive(false)하기)
        model.CurBuffDuration
           .Where(cur => cur <= 0)
           .Subscribe(_ =>
           {
               view.gameObject.SetActive(false);
           })
           .AddTo(disposable);
    }

    private Sprite GetBuffIcon()
    {
        if (model is BuffSkill)
            return model.SkillIcon;
        else
            return null;
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

}
