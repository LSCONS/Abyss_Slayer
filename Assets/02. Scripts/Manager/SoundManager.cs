using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서 그룹")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("사운드 데이터")]
    public SoundLibrary soundLibrary; // 사운드 라이브러리
    private Dictionary<EGameState, List<SoundData>> stateToSoundList = new();   // 어떤 상태가 무슨 사운드를 불러왔는지 추적할 수 있는 딕셔너리

    [Header("오디오 소스 풀")]
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // sfx 풀
    private List<AudioSource> activeSources = new List<AudioSource>(); // 활성화된 소스
    [SerializeField] private int poolSize = 10; // 풀 크기

    [Header("브금 소스")]
    private AudioSource[] bgmSources = new AudioSource[2];
    [SerializeField] private int currentBgmIndex = 0;
    private Coroutine bgmCoroutine; // 페이드인아웃에 쓸 브금 코루틴
    [SerializeField] private float bgmFadeTime = 1f; // 브금 페이드 시간

    [Header("볼륨")]
    private float masterVolume = 0f; // 마스터 볼륨
    private float bgmVolume = 0f; // 브금 볼륨
    private float sfxVolume = 0f; // sfx 볼륨

    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        LoadVolumeSettings();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="gameState"></param>
    /// <returns></returns>
    public async Task Init(EGameState gameState)
    {
        InitBGM();
        InitPool();
        soundLibrary = ScriptableObject.CreateInstance<SoundLibrary>();

        List<SoundData> loadedSounds = await soundLibrary.LoadSoundsByLabel(gameState);         // 씬 전용 SFX 라벨
        stateToSoundList[gameState] = loadedSounds;        // 딕셔너리에 저장
    }

    /// <summary>
    /// gameState 별로 언로드
    /// </summary>
    /// <param name="gameState"></param>
    public void UnloadSoundsByState(EGameState gameState)
    {
        if (!stateToSoundList.TryGetValue(gameState, out var soundList)) return;

        foreach (var sound in soundList)
        {
            sound.audioClip.ReleaseAsset();
            sound.cachedClip = null;
            soundLibrary.Remove(sound.soundName);
        }
        stateToSoundList.Remove(gameState);
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
    /// 이름으로 사운드 재생
    /// </summary>
    /// <param name="soundName">사운드 이름</param>
    public void PlaySound(string soundName)
    {
        var data = soundLibrary.GetSoundData(soundName);
        if (data == null) return;

        StartCoroutine(PlaySoundAsync(data));
    }

    /// <summary>
    /// 사운드 재생하는 코루틴  
    /// </summary>
    /// <param name="data">사운드 데이터</param>
    /// <returns></returns>
    private IEnumerator PlaySoundAsync(SoundData data)
    {
        if (data.cachedClip == null)
        {
            var handle = data.audioClip.LoadAssetAsync<AudioClip>();
            yield return handle;
            // 실제 재생
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                data.cachedClip = handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load audio clip: {data.soundName}");
                yield break;
            }
        }

        AudioSource audioSource = GetPooledSource();
        audioSource.clip = data.cachedClip;
        audioSource.volume = data.volume * sfxVolume * MasterVolume;
        audioSource.loop = data.loop;
        audioSource.Play();

        if (!data.loop)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            ReturnSource(audioSource);
        }
    }

    /// <summary>
    /// 풀에서 오디오 소스 가져오기
    /// </summary>
    /// <returns>오디오 소스</returns>
    private AudioSource GetPooledSource()
    {
        AudioSource src;
        if (sfxPool.Count > 0)
        {
            src = sfxPool.Dequeue();
        }
        else
        {
            src = CreateAudioSource();
        }

        src.gameObject.SetActive(true);
        activeSources.Add(src);
        return src;
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

    /// <summary>
    /// 모든 사운드 언로드
    /// </summary>
    public void UnloadAllSounds()
    {
        soundLibrary.UnloadAllSounds();
    }

    #endregion

    //==================== 브금 설정 ====================

    #region 브금 설정

    /// <summary>
    /// 브금 이름을 넣어서 브금 재생
    /// </summary>
    /// <param name="soundName">브금 이름</param>
    public void PlayBGM(string soundName)
    {
        var data = soundLibrary.GetSoundData(soundName);
        if (data == null) return;
        StartCoroutine(PlayBGMAsync(data)); // 브금 재생
    }

    /// <summary>
    /// 게임 스테이트에 따라서 playBGM 하는 오버로드 메서드
    /// </summary>
    /// <param name="gameState"></param>
    public void PlayBGM(EGameState gameState, int i)
    {
        string bgmName = $"BGM_{gameState}{i}";
        PlayBGM(bgmName);
    }

    /// <summary>
    /// 사운드 정지
    /// </summary>
    /// <param name="soundName">사운드 이름</param>
    public void StopSound(string soundName)
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            var src = activeSources[i];
            if (src.isPlaying && src.clip != null && src.clip.name == soundName)
            {
                src.Stop();
                ReturnSource(src);
                break;
            }
        }
    }


    private IEnumerator PlayBGMAsync(SoundData data)
    {
        if (data.cachedClip == null)
        {
            var handle = data.audioClip.LoadAssetAsync<AudioClip>();
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                data.cachedClip = handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load audio clip: {data.soundName}");
                yield break;
            }
        }

        if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(FadeInOutBGM(data.cachedClip, data.volume * bgmVolume));
    }

    /// <summary>
    /// 브금 페이드 인아웃
    /// </summary>
    /// <param name="newClip"></param>
    /// <returns></returns>
    private IEnumerator FadeInOutBGM(AudioClip newClip, float volume)
    {
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
        next.clip = newClip;
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
            src.volume = src.volume *sfxVolume * masterVolume;
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

