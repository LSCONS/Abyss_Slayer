using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIHealthBar : UIBase, IView
{
    [SerializeField] private Image hpBar;
    [SerializeField] private IPresenter presenter;


    private void OnValidate() {
        hpBar = transform.GetGameObjectSameNameDFS("HpBar").GetComponent<Image>();
    }

    public override void Init()
    {
        hpBar.fillAmount = 1;
    }

    public void SetHp(float value)
    {
        hpBar.fillAmount = value;
    }

    public void SetPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
    }

    private void OnDestroy()
    {
        presenter?.Dispose();
    }


}
