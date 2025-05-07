using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerPartsLoader : MonoBehaviour
{
    [SerializeField] private PlayerCustomPartsData playerPartsData;
    [SerializeField] private PlayerClassPartsData playerClassPartsData;
    [SerializeField] private CharacterClass playercharClass;
    private async void LoadPlayerParts()
    {
        // 플레이어 파츠 로드하기
        var hair = await Addressables.LoadAssetAsync<Sprite[]>(playerPartsData.hairKey).Task;
        var face = await Addressables.LoadAssetAsync<Sprite[]>(playerPartsData.faceKey).Task;
        var skin = await Addressables.LoadAssetAsync<Sprite[]>(playerPartsData.skinKey).Task;
    }
}
