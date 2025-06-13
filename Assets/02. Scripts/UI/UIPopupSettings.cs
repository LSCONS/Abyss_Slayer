using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopupSettings : UIPopup
{
    [Header("설정창 세팅")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_InputField masterInputField; 
    [SerializeField] private TMP_InputField bgmInputField; 
    [SerializeField] private TMP_InputField sfxInputField; 

    [SerializeField] private Button closeButton;

    private void OnValidate()
    {
#if AllMethodDebug
        Debug.Log("OnValidate");
#endif
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
#if AllMethodDebug
        Debug.Log("Init");
#endif
        base.Init();

       // 슬라이더 값 동기화
        masterSlider.value = ManagerHub.Instance.SoundManager.MasterVolume;
        bgmSlider.value = ManagerHub.Instance.SoundManager.BGMVolume;
        sfxSlider.value = ManagerHub.Instance.SoundManager.SFXVolume;

        // 믹서에 값 반영
        ManagerHub.Instance.SoundManager.SetMasterVolume(masterSlider.value);
        ManagerHub.Instance.SoundManager.SetBGMVolume(bgmSlider.value);
        ManagerHub.Instance.SoundManager.SetSFXVolume(sfxSlider.value);

        if(closeButton == null)
        closeButton = transform.GetGameObjectSameNameDFS("Close").GetComponent<Button>();
    }

    public override void Open(params object[] args)
    {
#if AllMethodDebug
        Debug.Log("Open");
#endif
        base.Open();

        // 슬라이더 값 동기화 (안전 위해?)
        masterSlider.value = ManagerHub.Instance.SoundManager.MasterVolume;
        bgmSlider.value = ManagerHub.Instance.SoundManager.BGMVolume;
        sfxSlider.value = ManagerHub.Instance.SoundManager.SFXVolume;

        masterSlider.onValueChanged.AddListener(val =>{
            ManagerHub.Instance.SoundManager.SetMasterVolume(val);
            masterInputField.text = Mathf.RoundToInt(val * 100).ToString();
        });

        bgmSlider.onValueChanged.AddListener(val =>{
            ManagerHub.Instance.SoundManager.SetBGMVolume(val);
            bgmInputField.text = Mathf.RoundToInt(val * 100).ToString();
        });

        sfxSlider.onValueChanged.AddListener(val =>{
            ManagerHub.Instance.SoundManager.SetSFXVolume(val);
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
#if AllMethodDebug
        Debug.Log("Close");
#endif
        base.Close();
        masterSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        masterInputField.onEndEdit.RemoveAllListeners();
        bgmInputField.onEndEdit.RemoveAllListeners();
        sfxInputField.onEndEdit.RemoveAllListeners();
    }

    public override void OnClose()
    {
#if AllMethodDebug
        Debug.Log("OnClose");
#endif
        ManagerHub.Instance.SoundManager.SaveVolumeSettings();
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        base.OnClose();
    }

    public void OnMasterSliderValueChanged(float value)
    {
#if AllMethodDebug
        Debug.Log("OnMasterSliderValueChanged");
#endif
        ManagerHub.Instance.SoundManager.SetMasterVolume(value);
    }

    public void OnBGMVolumeSliderValueChanged(float value)
    {
#if AllMethodDebug
        Debug.Log("OnBGMVolumeSliderValueChanged");
#endif
        ManagerHub.Instance.SoundManager.SetBGMVolume(value);
    }

    public void OnSFXVolumeSliderValueChanged(float value)
    {
#if AllMethodDebug
        Debug.Log("OnSFXVolumeSliderValueChanged");
#endif
        ManagerHub.Instance.SoundManager.SetSFXVolume(value);
    }


    // 슬라이더 값 입력 필드 동기화
    private void SyncInputfilds()
    {
#if AllMethodDebug
        Debug.Log("SyncInputfilds");
#endif
        masterInputField.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        bgmInputField.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
        sfxInputField.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
    }

    private void OnMasterInputChanged(string value)
    {
#if AllMethodDebug
        Debug.Log("OnMasterInputChanged");
#endif
        if (float.TryParse(value, out float result))
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            masterSlider.value = float.Parse(value) / 100f;
            ManagerHub.Instance.SoundManager.SetMasterVolume(masterSlider.value);
        }
    }

    private void OnBGMInputChanged(string value)
    {
#if AllMethodDebug
        Debug.Log("OnBGMInputChanged");
#endif
        if (float.TryParse(value, out float result))
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            bgmSlider.value = float.Parse(value) / 100f;
            ManagerHub.Instance.SoundManager.SetBGMVolume(bgmSlider.value);
        }
    }

    private void OnSFXInputChanged(string value)
    {
#if AllMethodDebug
        Debug.Log("OnSFXInputChanged");
#endif
        if (float.TryParse(value, out float result))     
        {
            value = Mathf.Clamp(result, 0, 100).ToString();
            sfxSlider.value = float.Parse(value) / 100f;
            ManagerHub.Instance.SoundManager.SetSFXVolume(sfxSlider.value);
        }
    }

}
