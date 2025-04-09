using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBinder : MonoBehaviour
{
    private void Start() 
    {
        Bind<HPTest, UIHealthBar, HealthPresenter>("Player", "PlayerHp");
    }

    private void Bind<TModel, TView, TPresenter>(string targetName, string viewName)
    where TModel : Component
    where TView : IView
    where TPresenter : IPresenter
    {
        var model = GameObject.Find(targetName).GetComponent<TModel>();
        var view = GameObject.Find(viewName).GetComponent<TView>();
        var presenter = (IPresenter)System.Activator.CreateInstance(typeof(TPresenter), model, view);
        view.SetPresenter(presenter);
    }

}
