using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIButton : UIBase
{
    protected Button button;
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }
    public  override void Init()
    {
        base.Init();
    }
}
