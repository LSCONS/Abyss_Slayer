using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBinder : Singleton<UIBinder>
{
    // private void Start() 
    // {
    //     Bind<HPTest, UIHealthBar, HealthPresenter>("Player", "PlayerHp");
    // }

    /// <summary>
    /// 뷰와 프리젠터를 바인딩하는 메서드
    /// </summary>
    /// <typeparam name="TModel">모델 타입</typeparam>
    /// <typeparam name="TView">뷰 타입</typeparam>
    /// <typeparam name="TPresenter">프리젠터 타입</typeparam>
    /// <param name="targetName">모델 이름</param>
    /// <param name="viewName">뷰 이름</param>
    public void Bind<TModel, TView, TPresenter>(string targetName, string viewName)
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
