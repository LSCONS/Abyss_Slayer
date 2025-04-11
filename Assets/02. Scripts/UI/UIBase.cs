using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector.Libs;

[System.Flags]
public enum UIType
{
    None = 0,
    NonGamePlay = 1 << 0,
    GamePlay = 1 << 1,
    Permanent = 1 << 2,
    Top = 1 << 3,
    Bottom = 1 << 4,
    Popup = 1 << 5,
    All = ~0,
}

public abstract class UIBase : MonoBehaviour
{
    public UIType uiType;
    public abstract void Init();
    public virtual void Open(params object[] args)
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
