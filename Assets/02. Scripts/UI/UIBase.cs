using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    Default,   // 기본 형식
    Popup,   // 팝업 형식
    Scene,   // 상시 띄워져야 되는 ui
    Top,     // 상단에 띄워야 되는 ui
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
