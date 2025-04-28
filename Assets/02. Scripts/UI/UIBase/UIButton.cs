using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIButton : UIBase
{
    protected Button button;

    public  override void Init()
    {
        if(button == null) button = GetComponent<Button>();
        base.Init();
    }
}
