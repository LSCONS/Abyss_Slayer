using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject   
{
    public string soundName;
    public AssetReferenceT<AudioClip> audioClip;    // Addressables로 로드할 실제 AudioClip 주소
    public float volume = 1f;
    public bool loop = false;
    [System.NonSerialized]
    public AudioClip cachedClip;
}

