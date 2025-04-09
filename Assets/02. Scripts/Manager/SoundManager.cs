using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>
{
    public SoundLibrary soundLibrary; // 사운드 라이브러리
    private AudioSource[] bgmSources = new AudioSource[2];
    private int currentBgmIndex = 0;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // sfx 풀
    private List<AudioSource> activeSources = new List<AudioSource>(); // 활성화된 소스
    [SerializeField] private int poolSize = 10; // 풀 크기

    private float masterVolume = 0f; // 마스터 볼륨

    private float bgmVolume = 0f; // 브금 볼륨
    private float sfxVolume = 0f; // sfx 볼륨

    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    private float bgmFadeTime = 1f; // 브금 페이드 시간

    private Coroutine bgmCoroutine; // 페이드인아웃에 쓸 브금 코루틴

    /// <summary>
    /// 사운드라이브러리 로드 + 풀 초기화
    /// </summary>
    /// <returns></returns>
    public async Task Init(string sceneSfxLabel)
    {
        InitBGM();
        InitPool();
        soundLibrary = ScriptableObject.CreateInstance<SoundLibrary>();

        // 씬 전용 SFX 라벨
        await soundLibrary.LoadSoundsByLabel(sceneSfxLabel);

        // 공통 UI 사운드도 추가로 로드 가능
        // await soundLibrary.LoadSoundsByLabel("SFX_UI"); // 예시
    }

    /// <summary>
    /// 오디오소스 풀 초기화
    /// </summary>
    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var src = CreateAudioSource();
            src.gameObject.SetActive(false);
            sfxPool.Enqueue(src);
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
        return src;
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
    /// 이름으로 사운드 재생
    /// </summary>
    /// <param name="soundName">사운드 이름</param>
    public void PlaySound(string soundName)
    {
        var data = soundLibrary.GetSoundData(soundName);
        if (data == null)
        {
            return;
        }

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
        audioSource.volume = data.volume * sfxVolume;
        audioSource.loop = data.loop;
        audioSource.Play();

        if (!data.loop)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            ReturnSource(audioSource);
        }
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

    /// <summary>
    /// 모든 사운드 언로드
    /// </summary>
    public void UnloadAllSounds()
    {
        soundLibrary.UnloadAllSounds();
    }


    /// <summary>
    /// 브금 소스 초기화
    /// </summary>
    private void InitBGM()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject bgmObj = new GameObject($"BGM_AudioSource_{i}");
            bgmObj.transform.SetParent(this.transform);
            AudioSource src = bgmObj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.volume = 0f;
            bgmSources[i] = src;
        }
    }
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
        if (bgmSources[currentBgmIndex] != null)
            bgmSources[currentBgmIndex].volume = value;
    }

    // sfx 볼륨 설정
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        foreach (var src in activeSources)
        {
            if (src.clip != null)
            {
                var data = soundLibrary.GetSoundData(src.clip.name);
                if (data != null)
                    src.volume = data.volume * sfxVolume;
            }
        }
    }
    // 마스터 볼륨 설정 
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplyBGMVolume();
        ApplySFXVolume();
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

}

