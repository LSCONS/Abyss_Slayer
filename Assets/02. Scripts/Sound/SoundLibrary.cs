using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class SoundLibrary : ScriptableObject      // 오디오 클립을 관리하는 스크립터블 오브젝트
{
    private Dictionary<string, AudioClip> soundMap = new ();


    /// <summary>
    /// 오디오 클립 데이터 가져오기
    /// </summary>
    /// <param name="soundName">오디오 클립 이름</param>
    /// <returns>오디오 클립 데이터</returns>
    public AudioClip GetSoundClip(string soundName)
    {
        if (soundMap.TryGetValue(soundName, out var clip))
        {
            return clip;
        }
        return null;
    }

    /// <summary>
    /// 라벨을 기반으로 오디오 클립을 로드하고 라벨에 해당하는 오디오 클립을 모두 추가   
    /// </summary>
    /// <param name="soundKey">오디오 클립을 로드할 라벨</param>
    /// <returns>비동기 작업 결과</returns>
    public async Task<List<AudioClip>> LoadSoundsByLabel(EGameState soundKey)    // 게임 스테이트에 따라서 불러올거니까 그걸로 라벨 정기반
    {   
        var handle = Addressables.LoadAssetsAsync<AudioClip>(soundKey.ToString(), null);
        await handle.Task;

        List<AudioClip> loadedList = new(); //  반환용 리스트

        foreach (var sound in handle.Result)
        {
            if (!soundMap.ContainsKey(sound.name))
            {
                soundMap[sound.name] = sound;
                loadedList.Add(sound);
                Debug.Log($"[SoundLibrary] Loaded: {sound.name}");
            }
        }

        return loadedList;
    }

    /// <summary>
    /// 모든 오디오 클립을 언로드
    /// </summary>
    public void UnloadAllSounds()
    {
        soundMap.Clear();
    }


    /// <summary>
    /// 하나하나 등록
    /// </summary>
    /// <param name="sound"></param>
    public void Add(AudioClip sound)
    {
        if (!soundMap.ContainsKey(sound.name))
        {
            soundMap[sound.name] = sound;
        }
    }

    /// <summary>
    /// 언로드하면 사운드 제거
    /// </summary>
    /// <param name="soundName"></param>
    public void Remove(string soundName)
    {
        if(soundMap.TryGetValue(soundName, out var sound))
        {
            soundMap.Remove(soundName);
        }
    }

    /// <summary>
    /// 이름으로 있는지 확인
    /// </summary>
    /// <param name="soundName"></param>
    /// <returns></returns>
    public bool Contains(string soundName)
    {
        return soundMap.ContainsKey(soundName);
    }

}
