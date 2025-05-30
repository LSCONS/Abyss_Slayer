using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IView
{
    void SetPresenter(IPresenter presenter);
}

public interface IPresenter
{
    void Dispose();
}