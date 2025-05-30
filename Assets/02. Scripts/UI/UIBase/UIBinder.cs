using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIBinder
{

    /// <summary>
    /// 뷰와 프리젠터를 바인딩하는 메서드
    /// </summary>
    /// <typeparam name="TModel">모델 타입</typeparam>
    /// <typeparam name="TView">뷰 타입</typeparam>
    /// <typeparam name="TPresenter">프리젠터 타입</typeparam>
    /// <param name="targetName">모델 이름</param>
    /// <param name="viewName">뷰 이름</param>
    public static void Bind<TModel, TView, TPresenter>(string targetName, string viewName)
    where TModel : IHasHealth
    where TView : IView
    where TPresenter : IPresenter
    {
        var model = GameObject.Find(targetName).GetComponent<TModel>();
        var view = GameObject.Find(viewName).GetComponent<TView>();
        var presenter = (IPresenter)System.Activator.CreateInstance(typeof(TPresenter), model, view);
        view.SetPresenter(presenter);
    }

    /// <summary>
    /// 뷰와 프리젠터를 바인딩하는 메서드
    /// </summary>
    /// <typeparam name="TModel">모델 타입</typeparam>
    /// <typeparam name="TView">뷰 타입</typeparam>
    /// <typeparam name="TPresenter">프리젠터 타입</typeparam>
    /// <param name="targetName">모델 이름</param>
    /// <param name="_view">뷰 오브젝트</param>
    public static bool Bind<TView, TPresenter>(IHasHealth health, GameObject _view)
    where TView : IView
    where TPresenter : IPresenter
    {
        var view = _view.GetComponent<TView>();
        if (view == null)
        {
            Debug.LogWarning($"[UIBinder] {_view.name}에 {typeof(TView)} 컴포넌트가 없다");
            return false;
        }

        var presenter = (IPresenter)System.Activator.CreateInstance(typeof(TPresenter), health, view);
        view.SetPresenter(presenter);

        return true;
    }

    public static bool BindBoss<TModel, TView, TPresenter>(Boss boss, GameObject _view)
where TModel : IHasHealth
where TView : IView
where TPresenter : IPresenter
    {
        if (boss is not IHasHealth model)
        {
            Debug.LogWarning($"[UIBinder] {boss.name}에 {typeof(TModel)} 컴포넌트가 없다");
            return false;
        }
        if (!(Bind<TView, TPresenter>(model, _view))) return false;

        return true;
    }


    public static bool BindPlayer<TModel, TView, TPresenter>(Player player, GameObject _view)
where TModel : IHasHealth
where TView : IView
where TPresenter : IPresenter
    {
        if (player is not TModel model)
        {
            Debug.LogWarning($"[UIBinder] Player에 {typeof(TModel)} 컴포넌트가 없다");
            return false;
        }

        if (!(Bind<TView, TPresenter>(model, _view))) return false;

        return true;
    }
}
