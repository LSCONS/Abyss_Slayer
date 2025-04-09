using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSettings : UIPopup
{
    [SerializeField] private Slider masterSlider;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;


    private void OnValidate()
    {
        // 자식 오브젝트로 달려있는 슬라이더들을 찾아서 자동으로 연결
        masterSlider = transform.GetGameObjectSameNameDFS("Master Volume").GetComponent<Slider>();
        bgmSlider = transform.GetGameObjectSameNameDFS("BGM Volume").GetComponent<Slider>();
        sfxSlider = transform.GetGameObjectSameNameDFS("SFX Volume").GetComponent<Slider>();
    }
    public override void Init()
    {
        base.Init();

        // 슬라이더 값 동기화
        masterSlider.value = SoundManager.Instance.MasterVolume;
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;
    }

    public override void Open(params object[] args)
    {
        base.Open();

        // 슬라이더 값 동기화 (안전 위해?)
        masterSlider.value = SoundManager.Instance.MasterVolume;
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;


        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);

        
    }

    public override void Close()
    {
        base.Close();
        masterSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);
    }

    public void OnMasterSliderValueChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    public void OnBGMVolumeSliderValueChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void OnSFXVolumeSliderValueChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
