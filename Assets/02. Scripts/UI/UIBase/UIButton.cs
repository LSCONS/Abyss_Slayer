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
        button?.onClick.RemoveListener(PlayClickSound);
        button?.onClick.AddListener(PlayClickSound);
    }

    protected virtual void PlayClickSound()
    {
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
    }
}
