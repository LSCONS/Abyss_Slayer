using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>
{

    public SoundLibrary soundLibrary;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
    private List<AudioSource> activeSources = new List<AudioSource>();
    [SerializeField] private int poolSize = 10;



    /// <summary>
    /// 사운드라이브러리 로드 + 풀 초기화
    /// </summary>
    /// <returns></returns>
    public async Task Init(string sceneSfxLabel)
    {
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
        audioSource.volume = data.volume;
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

}

