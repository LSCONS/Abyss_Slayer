using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

[Serializable]
public class SoundManager
{
    [field: Header("오디오 믹서 그룹")]
    [field: SerializeField] private AudioMixer AudioMixer { get; set; }
    [field: SerializeField] private AudioMixerGroup BGMGroup { get; set; }
    [field: SerializeField] private AudioMixerGroup SFXGroup { get; set; }
    private readonly Dictionary<EAudioClip, AudioSource> loopingSources = new();    // 현재 루프 중인 클립 저장


    [field: Header("오디오 소스 풀")]
    [field: SerializeField] private int PoolSize { get; set; } = 10; // 풀 크기
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();      // sfx 풀
    private List<AudioSource> activeSources = new List<AudioSource>();  // 활성화된 소스

    [field: Header("브금 소스")]
    [field: SerializeField] private int CurrentBgmIndex { get; set; } = 0;
    private AudioSource[] bgmSources = new AudioSource[2];
    private Coroutine bgmCoroutine;                         // 페이드인아웃에 쓸 브금 코루틴
    [field: SerializeField] private float BGMFadeTime { get; set; } = 1f;        // 브금 페이드 시간

    [field:Header("볼륨")]
    [field: SerializeField] public float MasterVolume { get; private set; } = 0f;    // 마스터 볼륨
    [field: SerializeField] public float BGMVolume { get; private set; } = 0f;       // 브금 볼륨
    [field: SerializeField] public float SFXVolume { get; private set; } = 0f;       // sfx 볼륨

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
        for (int i = 0; i < PoolSize; i++)
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
            bgmObj.transform.SetParent(ManagerHub.Instance.transform);
            AudioSource src = bgmObj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0f;
            src.outputAudioMixerGroup = BGMGroup;          // 오디오 믹서 그룹 연결
            bgmSources[i] = src;
        }
    }

    public void PlayTypingSoundSFX()
    {
        PlayRandomPitchSFX
        (
            EAudioClip.SFX_KeyBoardTyping,
            ManagerHub.Instance.GameValueManager.MinTypingSoundPitch,
            ManagerHub.Instance.GameValueManager.MaxTypingSoundPitch
        );
    }

    public async Task PlayBGMIntro()
    {
        var data = Addressables.LoadAssetAsync<AudioClipDataGather>("Intro");
        await data.Task;

        AudioClipDataGather gatherData = data.Result;
        foreach (var datas in gatherData.ListAudioClipEnumData)
        {
            ManagerHub.Instance.DataManager.DictEnumToAudioData[datas.EnumClip] = datas.AudioClipData;
        }
        PlayBGM(EAudioClip.BGM_IntroScene);
        return;
    }

    /// <summary>
    /// 오디오 소스 생성
    /// </summary>
    /// <returns>오디오 소스</returns>
    private AudioSource CreateAudioSource()
    {
        GameObject go = new GameObject("SFX_AudioSource");
        go.transform.SetParent(ManagerHub.Instance.transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.outputAudioMixerGroup = SFXGroup;               // 오디오 믹서 그룹 연결
        return src;
    }

    /// <summary>
    /// 풀 초기화
    /// </summary>
    public void ClearPool()
    {
        foreach(var src in activeSources)
        {
            GameObject.Destroy(src);
        }
        while (sfxPool.Count > 0)
        {
            GameObject.Destroy(sfxPool.Dequeue());
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
        if (!(ManagerHub.Instance.DataManager.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData data))) return;
        if(data.Audio == null) return;

        PlaySound(EAudioClip, data);
    }
    public void PlayRandomPitchSFX(EAudioClip EAudioClip, float minPitch, float maxPitch)
    {
        if (!(ManagerHub.Instance.DataManager.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData data))) return;
        if (data.Audio == null) return;
        float ptich = UnityEngine.Random.Range(minPitch, maxPitch);
        PlayRandomPitchSound(EAudioClip, data, ptich);
    }


    private void PlayRandomPitchSound(EAudioClip EAudioClip, AudioClipData data, float pitch)
    {
        if (data.Audio == null) return;

        var src = GetPooledSource();
        src.clip = data.Audio;
        src.loop = data.IsLoop;
        src.pitch = pitch;
        src.volume = SFXVolume * MasterVolume * data.Volume;
        src.Play();

        if (!data.IsLoop)
        {
            ManagerHub.Instance.StartCoroutine(ReturnWhenDone(src));
        }
        else
        {
            loopingSources[EAudioClip] = src;  // 루프면 딕셔너리에 저장
        }
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
        src.volume = SFXVolume * MasterVolume * data.Volume;
        src.Play();

        if (!data.IsLoop)
        {
            ManagerHub.Instance.StartCoroutine(ReturnWhenDone(src));
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
        if (!(ManagerHub.Instance.DataManager.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData audioData))) return;
        if (audioData.Audio == null) return;

        //TODO: 나중에 BGM에 pitch, volume 등 포함

        if (bgmCoroutine != null) ManagerHub.Instance.StopCoroutine(bgmCoroutine);
        bgmCoroutine = ManagerHub.Instance.StartCoroutine(FadeInOutBGM(audioData));
    }

    /// <summary>
    /// 클립 이름으로 사운드 정지
    /// </summary>
    /// <param name="soundName">멈출 사운드 이름</param>
    public void StopSound(EAudioClip EAudioClip)
    {
        if(!(ManagerHub.Instance.DataManager.DictEnumToAudioData.TryGetValue(EAudioClip, out AudioClipData audioData))) return;
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
        float volume = BGMVolume * MasterVolume * newClip.Volume;

        int nextIndex = 1 - CurrentBgmIndex;
        AudioSource current = bgmSources[CurrentBgmIndex];
        AudioSource next = bgmSources[nextIndex];

        // 페이드 아웃
        float time = 0f;
        float startVolume = current.volume;
        while (time < BGMFadeTime)
        {
            current.volume = Mathf.Lerp(startVolume, 0f, time / BGMFadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        current.Stop();

        // 페이드 인
        next.clip = newClip.Audio;
        next.pitch = newClip.Pitch;
        next.Play();
        time = 0f;
        while (time < BGMFadeTime)
        {
            next.volume = Mathf.Lerp(0f, volume, time / BGMFadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        next.volume = volume;
        CurrentBgmIndex = nextIndex;
    }

    // 브금 볼륨 설정
    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        AudioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    // sfx 볼륨 설정
    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }
    // 마스터 볼륨 설정 
    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
    }

    // 브금 볼륨 적용
    private void ApplyBGMVolume()
    {
        bgmSources[CurrentBgmIndex].volume = BGMVolume * MasterVolume;
    }

    // sfx 볼륨 적용
    private void ApplySFXVolume()
    {
        foreach (var src in activeSources)
        {
            src.volume = SFXVolume * MasterVolume;
        }
    }

    #endregion

    /// <summary>
    /// 볼륨 설정 저장 
    /// </summary>
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(PlayerPrefabData.Volume_Master, MasterVolume);
        PlayerPrefs.SetFloat(PlayerPrefabData.Volume_BGM, BGMVolume);
        PlayerPrefs.SetFloat(PlayerPrefabData.Volume_SFX, SFXVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 볼륨 설정 불러오기
    /// </summary>
    public void LoadVolumeSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat(PlayerPrefabData.Volume_Master, 0.5f); // 기본값 1
        BGMVolume = PlayerPrefs.GetFloat(PlayerPrefabData.Volume_BGM, 1f);
        SFXVolume = PlayerPrefs.GetFloat(PlayerPrefabData.Volume_SFX, 1f);

        // AudioMixer에도 즉시 반영
        SetMasterVolume(MasterVolume);
        SetBGMVolume(BGMVolume);
        SetSFXVolume(SFXVolume);
    }
}

