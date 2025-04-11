using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopupSettings : UIPopup
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_InputField masterInputField; 
    [SerializeField] private TMP_InputField bgmInputField; 
    [SerializeField] private TMP_InputField sfxInputField; 


    [SerializeField] private Button closeButton;


    private void OnValidate()
    {
        // 자식 오브젝트로 달려있는 슬라이더들을 찾아서 자동으로 연결
        masterSlider = transform.GetGameObjectSameNameDFS("Master Volume").GetComponent<Slider>();
        bgmSlider = transform.GetGameObjectSameNameDFS("BGM Volume").GetComponent<Slider>();
        sfxSlider = transform.GetGameObjectSameNameDFS("SFX Volume").GetComponent<Slider>();

        masterInputField = transform.GetGameObjectSameNameDFS("Master Volume InputField").GetComponent<TMP_InputField>();
        bgmInputField = transform.GetGameObjectSameNameDFS("BGM Volume InputField").GetComponent<TMP_InputField>();
        sfxInputField = transform.GetGameObjectSameNameDFS("SFX Volume InputField").GetComponent<TMP_InputField>();


       
    }
    public override void Init()
    {
        base.Init();

        // 슬라이더 값 동기화
        masterSlider.value = SoundManager.Instance.MasterVolume;
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;

        closeButton = transform.GetGameObjectSameNameDFS("Close").GetComponent<Button>();
    }

    public override void Open(params object[] args)
    {
        base.Open();

        // 슬라이더 값 동기화 (안전 위해?)
        masterSlider.value = SoundManager.Instance.MasterVolume;
        bgmSlider.value = SoundManager.Instance.BGMVolume;
        sfxSlider.value = SoundManager.Instance.SFXVolume;


        // masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        // bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        // sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);

        masterSlider.onValueChanged.AddListener(val =>{
            SoundManager.Instance.SetMasterVolume(val);
            masterInputField.text = Mathf.RoundToInt(val * 100).ToString();
        });

        bgmSlider.onValueChanged.AddListener(val =>{
            SoundManager.Instance.SetBGMVolume(val);
            bgmInputField.text = Mathf.RoundToInt(val * 100).ToString();
        });

        sfxSlider.onValueChanged.AddListener(val =>{
            SoundManager.Instance.SetSFXVolume(val);
            sfxInputField.text = Mathf.RoundToInt(val * 100).ToString();
        });

        masterInputField.onEndEdit.AddListener(OnMasterInputChanged);
        bgmInputField.onEndEdit.AddListener(OnBGMInputChanged);
        sfxInputField.onEndEdit.AddListener(OnSFXInputChanged);

        closeButton.onClick.AddListener(OnClose);

        SyncInputfilds();
    }

    public override void Close()
    {
        base.Close();
        masterSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);

        masterInputField.onEndEdit.RemoveAllListeners();
        bgmInputField.onEndEdit.RemoveAllListeners();
        sfxInputField.onEndEdit.RemoveAllListeners();
    }

    public override void OnClose()
    {
        Debug.Log("OnClose");
        base.OnClose();
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


    // 슬라이더 값 입력 필드 동기화
    private void SyncInputfilds()
    {
        masterInputField.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        bgmInputField.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
        sfxInputField.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
    }

    private void OnMasterInputChanged(string value)
    {
        if(float.TryParse(value, out float result))
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            masterSlider.value = float.Parse(value) / 100f;
            SoundManager.Instance.SetMasterVolume(masterSlider.value);
        }
    }

    private void OnBGMInputChanged(string value)
    {
        if(float.TryParse(value, out float result))
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            bgmSlider.value = float.Parse(value) / 100f;
            SoundManager.Instance.SetBGMVolume(bgmSlider.value);
        }
    }

    private void OnSFXInputChanged(string value)
    {
        if(float.TryParse(value, out float result))     
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            sfxSlider.value = float.Parse(value) / 100f;
            SoundManager.Instance.SetSFXVolume(sfxSlider.value);
        }
    }

}
