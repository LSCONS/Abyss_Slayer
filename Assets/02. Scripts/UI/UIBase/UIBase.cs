using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum UIType
{
    None = 0,
    NonGamePlay = 1 << 0,
    GamePlay = 1 << 1,
    Permanent = 1 << 2,
    Top = 1 << 3,
    Bottom = 1 << 4,
    TopMid = 1 << 5,
    Follow = 1 << 6,
    Popup = 1 << 7,
    Background = 1 << 8,
    Everything = NonGamePlay | GamePlay | Permanent
               | Top | Bottom | TopMid | Follow | Popup | Background
}

[System.Flags]
public enum UISceneType
{
    None = 0,
    Intro = 1 << 0,
    Start = 1 << 1,
    Lobby = 1 << 2,
    Boss = 1 << 3,
    Rest = 1 << 4,
    Loading = 1 << 5,
}

public abstract class UIBase : MonoBehaviour
{
    public UIType uiType;
    public UISceneType uISceneType = UISceneType.None;
    public virtual void Init()
    {
        UIManager.Instance.DelayRebuildLayout(this);    // 강제 정렬
        gameObject.SetActive(false);
    }
    public virtual void Open(params object[] args)
    {
        gameObject.SetActive(true);
        UIManager.Instance.DelayRebuildLayout(this);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
