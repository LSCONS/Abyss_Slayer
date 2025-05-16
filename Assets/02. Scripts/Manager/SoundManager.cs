using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서 그룹")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("사운드 데이터")]
    public SoundLibrary soundLibrary; // 사운드 라이브러리
    private Dictionary<EGameState, List<AudioClip>> stateToSoundList = new();   // 어떤 상태가 무슨 사운드를 불러왔는지 추적할 수 있는 딕셔너리
    private readonly Dictionary<string, AudioSource> loopingSources = new();    // 현재 루프 중인 클립 저장


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

        List<AudioClip> loadedSounds = await soundLibrary.LoadSoundsByLabel(gameState);         // 씬 전용 SFX 라벨
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
            Addressables.Release(sound);
            soundLibrary.Remove(sound.name);
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
    /// 효과음 재생 (클립이 SFX_타입 형식으로 어드레서블 등록 되어있어야됨)
    /// </summary>
    /// <param name="sfxType">효과음 타입</param>
    /// <param name="loop">효과음 루프 할 건지</param>
    /// <param name="pitch">효과음 속도</param>
    /// <param name="volum">효과음 크기</param>
    /// 
    public void PlaySFX(ESFXType sfxType, bool loop = false, float pitch = 1f, float volum = 1f)
    {
        string clipName = $"SFX_{sfxType}";
        PlaySound(clipName, loop, pitch, volum);
    }
    /// <summary>
    /// 효과음 재생 (직접 재생해줌)
    /// </summary>
    /// <param name="sfxType">효과음 타입</param>
    /// <param name="loop">효과음 루프 할 건지</param>
    /// <param name="pitch">효과음 속도</param>
    /// <param name="volum">효과음 크기</param>
    public void PlaySFX(AudioClip clip, bool loop = false, float pitch = 1f, float volum = 1f)
    {
        if (clip == null) return;

        // 직접 재생 방식으로
        var src = GetPooledSource();               
        src.clip = clip;
        src.loop = loop;
        src.pitch = pitch;
        src.volume = sfxVolume * masterVolume * volum;
        src.Play();

        if (!loop)
            StartCoroutine(ReturnWhenDone(src));
    }

    /// <summary>
    /// 이름으로 사운드 재생
    /// </summary>
    /// <param name="clipKey"></param>
    /// <param name="loop">루프 할 건지</param>
    /// <param name="pitch">효과음 속도</param>
    /// <param name="volum">효과음 크기</param>
    private async void PlaySound(string clipKey, bool loop = false, float pitch = 1f, float volum = 1f)
    {
        // 이미 루프중인 사운드면 리턴
        if (loop && loopingSources.ContainsKey(clipKey)) return;

        AudioClip clip = soundLibrary.GetSoundClip(clipKey);    // 라이브러리에 캐싱 된 클립을 먼저 시도

        // 없으면 다시 로드
        if (clip == null)
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(clipKey);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                clip = handle.Result;
            }
        }

        if (clip == null) return;

        var src = GetPooledSource();
        src.clip = clip;
        src.loop = loop;
        src.pitch = pitch;
        src.volume *= masterVolume * volum;
        src.Play();

        if (!loop)
        {
            StartCoroutine(ReturnWhenDone(src));
        }
        else
        {
            loopingSources[clipKey] = src;  // 루프면 딕셔너리에 저장
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
        var clip = soundLibrary.GetSoundClip(soundName);
        if (clip == null) return;

        if (bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(FadeInOutBGM(clip));
    }

    /// <summary>
    /// 게임 스테이트에 따라서 playBGM 하는 오버로드 메서드
    /// </summary>
    /// <param name="gameState">어떤 게임 스테이트의 브금을 로드할 건지</param>
    public void PlayBGM(EGameState gameState, int i)
    {
        string bgmName = $"BGM_{gameState}{i}";
        PlayBGM(bgmName);
    }

    /// <summary>
    /// 클립 이름으로 사운드 정지
    /// </summary>
    /// <param name="soundName">멈출 사운드 이름</param>
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

    /// <summary>
    /// 효과음 타입으로 사운드 정지
    /// </summary>
    /// <param name="type">효과음 타입</param>
    public void StopSFX(ESFXType type)
    {
        string soundName = $"SFX_{type}";

        if(loopingSources.TryGetValue(soundName, out var src))
        {
            src.Stop();
            ReturnSource(src);
            loopingSources.Remove(soundName);
            return;
        }
        else StopSound(soundName);
    }

    // 오디오 클리븡로 사운드 정지
    public void StopSFX(AudioClip clip)
    {
        if(clip == null) return;
        StopSound(clip.name);
    }

    /// <summary>
    /// 브금 페이드 인아웃
    /// </summary>
    /// <param name="newClip">다음에 재생 시킬 오디오 클립</param>
    /// <returns></returns>
    private IEnumerator FadeInOutBGM(AudioClip newClip)
    {
        float volume = bgmVolume * masterVolume;

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

