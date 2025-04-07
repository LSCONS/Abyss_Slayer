using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class SoundLibrary : ScriptableObject      // 오디오 클립을 관리하는 스크립터블 오브젝트
{
    private Dictionary<string, SoundData> soundMap = new Dictionary<string, SoundData>();


    /// <summary>
    /// 오디오 클립 데이터 가져오기
    /// </summary>
    /// <param name="soundName">오디오 클립 이름</param>
    /// <returns>오디오 클립 데이터</returns>
    public SoundData GetSoundData(string soundName)
    {
        if (soundMap.TryGetValue(soundName, out var data))
        {
            return data;
        }
        return null;
    }

    /// <summary>
    /// 라벨을 기반으로 오디오 클립을 로드하고 라벨에 해당하는 오디오 클립을 모두 추가   
    /// </summary>
    /// <param name="label">오디오 클립을 로드할 라벨</param>
    /// <returns>비동기 작업 결과</returns>
    public async Task LoadSoundsByLabel(string label)
    {   
        var handle = Addressables.LoadAssetsAsync<SoundData>(label, null);
        await handle.Task;
        
        foreach (var sound in handle.Result)
        {
            if (!soundMap.ContainsKey(sound.soundName))
            {
                soundMap[sound.soundName] = sound;
                Debug.Log($"[SoundLibrary] Loaded: {sound.soundName}");
            }
        }
    }

    /// <summary>
    /// 모든 오디오 클립을 언로드
    /// </summary>
    public void UnloadAllSounds()
    {
        foreach (var sound in soundMap.Values)
        {
            sound.audioClip.ReleaseAsset();
            sound.cachedClip = null;
        }
        soundMap.Clear();
    }
}
