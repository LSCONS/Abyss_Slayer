using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서 그룹")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    private Dictionary<ESceneName, List<AudioClip>> stateToSoundList { get; set; } = new();   // 어떤 상태가 무슨 사운드를 불러왔는지 추적할 수 있는 딕셔너리
    private readonly Dictionary<EAudioClip, AudioSource> loopingSources = new();    // 현재 루프 중인 클립 저장


    [Header("오디오 소스 풀")]
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();      // sfx 풀
    private List<AudioSource> activeSources = new List<AudioSource>();  // 활성화된 소스
    [SerializeField] private int poolSize = 10; // 풀 크기

    [Header("브금 소스")]
    private AudioSource[] bgmSources = new AudioSource[2];
    [SerializeField] private int currentBgmIndex = 0;
    private Coroutine bgmCoroutine;                         // 페이드인아웃에 쓸 브금 코루틴
    [SerializeField] private float bgmFadeTime = 1f;        // 브금 페이드 시간

    [Header("볼륨")]
    [SerializeField] private float masterVolume = 0f;    // 마스터 볼륨
    [SerializeField] private float bgmVolume = 0f;       // 브금 볼륨
    [SerializeField] private float sfxVolume = 0f;       // sfx 볼륨

    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;



    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="gameState"></param>
    /// <returns></returns>
    public void Init()
    {
        LoadVolumeSettings();
        InitBGM();
        InitPool();
    }

    //==================== 오디오 소스 풀 ====================

    #region 오디오 소스 풀
    /// <summary>
    /// 오디오소스 풀 초기화
    /// </summary>
    private void InitPool()
    {
        if (sfxPool.Count > 0) return;
        for (int i = 0; i < poolSize; i++)
        {
            var src = CreateAudioSource();
            src.gameObject.SetActive(false);
            sfxPool.Enqueue(src);
        }
    }

    /// <summary>
    /// 브금 소스 초기화
    /// </summary>
    private void InitBGM()
    {
        if (bgmSources[0] != null && bgmSources[1] != null) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject bgmObj = new GameObject($"BGM_AudioSource_{i}");
            bgmObj.transform.SetParent(this.transform);
            AudioSource src = bgmObj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0f;
            src.outputAudioMixerGroup = bgmGroup;          // 오디오 믹서 그룹 연결
            bgmSources[i] = src;
        }
    }

    public async void PlayBGMIntro()
    {
        var data = Addressables.LoadAssetAsync<AudioClipDataGather>("Intro");
        await data.Task;

        AudioClipDataGather gatherData = data.Result;
        foreach (var datas in gatherData.ListAudioClipEnumData)
        {
            DataManager.Instance.DictEnumToAudioData[datas.EnumClip] = datas.AudioClipData;
        }
        PlayBGM(EAudioClip.BGM_IntroScene);
    }

    /// <summary>
    /// 오디오 소스 생성
    /// </summary>
    /// <returns>오디오 소스</returns>
    private AudioSource CreateAudioSource()
    {
        GameObject go = new GameObject("SFX_AudioSource");
        go.transform.SetParent(this.transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.outputAudioMixerGroup = sfxGroup;               // 오디오 믹서 그룹 연결
        return src;
    }

    /// <summary>
    /// 풀 초기화
    /// </summary>
    public void ClearPool()
    {
        foreach(var src in activeSources)
        {
            Destroy(src);
        }
        while (sfxPool.Count > 0)
        {
            Destroy(sfxPool.Dequeue());
        }
        activeSources.Clear();
    }

    #endregion


    //==================== SFX 재생 ====================
    #region SFX 재생

    /// <summary>
    /// 효과음 재생 (클립이 SFX_타입 형식으로 어드레서블 등록 되어있어야됨)
    /// </summary>
    /// <param name="sfxType">효과음 타입</param>
    /// <param name="loop">효과음 루프 할 건지</param>
    /// <param name="pitch">효과음 속도</param>
    /// <param name="volum">효과음 크기</param>
    /// 
    public void PlaySFX(EAudioClip EAudioClip)
    {
        if (!(DataManager.Instance.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData data))) return;
        if(data.Audio == null) return;

        PlaySound(EAudioClip, data);
    }


    /// <summary>
    /// Enum으로 사운드 재생
    /// </summary>
    private void PlaySound(EAudioClip EAudioClip, AudioClipData data)
    {
        if (data.Audio == null) return;

        var src = GetPooledSource();
        src.clip = data.Audio;
        src.loop = data.IsLoop;
        src.pitch = data.Pitch;
        src.volume = sfxVolume * masterVolume * data.Volume;
        src.Play();

        if (!data.IsLoop)
        {
            StartCoroutine(ReturnWhenDone(src));
        }
        else
        {
            loopingSources[EAudioClip] = src;  // 루프면 딕셔너리에 저장
        }
    }

    /// <summary>
    /// 끝나면 다시 돌려놓기
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private IEnumerator ReturnWhenDone(AudioSource src)
    {
        yield return new WaitUntil(() => !src.isPlaying);
        ReturnSource(src);
    }

    /// <summary>
    /// 풀에서 오디오 소스 가져오기
    /// </summary>
    /// <returns>오디오 소스</returns>
    private AudioSource GetPooledSource()
    {
        int count = sfxPool.Count;

        // 다 돌면서 재생 중 아닌것들 찾아서 꺼냄
        for (int i = 0; i < count; i++)
        {
            var src = sfxPool.Dequeue();
            if (!src.isPlaying)
            {
                src.gameObject.SetActive(true);
                activeSources.Add(src);
                return src;
            }
            else sfxPool.Enqueue(src); // 아직 재생 중
        }
        // 만약에 다 재생중이면 풀 더 생성
        var newSrc = CreateAudioSource();
        newSrc.gameObject.SetActive(true);
        activeSources.Add(newSrc);
        return newSrc;
    }

    /// <summary>
    /// 오디오 소스 풀로 반환
    /// </summary>
    /// <param name="src"></param>
    private void ReturnSource(AudioSource src)
    {
        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);
        activeSources.Remove(src);
        sfxPool.Enqueue(src);
    }

    #endregion

    //==================== 브금 설정 ====================

    #region 브금 설정

    /// <summary>
    /// 브금 이름을 넣어서 브금 재생
    /// </summary>
    /// <param name="soundName">브금 이름</param>
    public void PlayBGM(EAudioClip EAudioClip)
    {
        if (!(DataManager.Instance.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData audioData))) return;
        if (audioData.Audio == null) return;

        //TODO: 나중에 BGM에 pitch, volume 등 포함

        if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(FadeInOutBGM(audioData));
    }

    /// <summary>
    /// 클립 이름으로 사운드 정지
    /// </summary>
    /// <param name="soundName">멈출 사운드 이름</param>
    public void StopSound(EAudioClip EAudioClip)
    {
        if(!(DataManager.Instance.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData audioData))) return;
        if(audioData.Audio == null) return;

        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var src = activeSources[i];
            if (src.isPlaying && src.clip != null && src.clip == audioData.Audio)
            {
                src.Stop();
                ReturnSource(src);
                break;
            }
        }
    }

    /// <summary>
    /// 효과음 타입으로 사운드 정지
    /// </summary>
    /// <param name="type">효과음 타입</param>
    public void StopSFX(EAudioClip EAuidioClip)
    {
        if(loopingSources.TryGetValue(EAuidioClip, out var src))
        {
            src.Stop();
            ReturnSource(src);
            loopingSources.Remove(EAuidioClip);
            return;
        }
        else StopSound(EAuidioClip);
    }

    /// <summary>
    /// 브금 페이드 인아웃
    /// </summary>
    /// <param name="newClip">다음에 재생 시킬 오디오 클립</param>
    /// <returns></returns>
    private IEnumerator FadeInOutBGM(AudioClipData newClip)
    {
        float volume = bgmVolume * masterVolume * newClip.Volume;

        int nextIndex = 1 - currentBgmIndex;
        AudioSource current = bgmSources[currentBgmIndex];
        AudioSource next = bgmSources[nextIndex];

        // 페이드 아웃
        float time = 0f;
        float startVolume = current.volume;
        while (time < bgmFadeTime)
        {
            current.volume = Mathf.Lerp(startVolume, 0f, time / bgmFadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        current.Stop();

        // 페이드 인
        next.clip = newClip.Audio;
        next.pitch = newClip.Pitch;
        next.Play();
        time = 0f;
        while (time < bgmFadeTime)
        {
            next.volume = Mathf.Lerp(0f, volume, time / bgmFadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        next.volume = volume;
        currentBgmIndex = nextIndex;
    }

    // 브금 볼륨 설정
    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    // sfx 볼륨 설정
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }
    // 마스터 볼륨 설정 
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    // 브금 볼륨 적용
    private void ApplyBGMVolume()
    {
        bgmSources[currentBgmIndex].volume = bgmVolume * masterVolume;
    }

    // sfx 볼륨 적용
    private void ApplySFXVolume()
    {
        foreach (var src in activeSources)
        {
            src.volume = sfxVolume * masterVolume;
        }
    }

    #endregion

    /// <summary>
    /// 볼륨 설정 저장 
    /// </summary>
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("Volume_Master", masterVolume);
        PlayerPrefs.SetFloat("Volume_BGM", bgmVolume);
        PlayerPrefs.SetFloat("Volume_SFX", sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 볼륨 설정 불러오기
    /// </summary>
    public void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("Volume_Master", 1f); // 기본값 1
        bgmVolume = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        sfxVolume = PlayerPrefs.GetFloat("Volume_SFX", 1f);

        // AudioMixer에도 즉시 반영
        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

}

