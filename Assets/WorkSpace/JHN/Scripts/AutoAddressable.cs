#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressablesAutoRegister
{
    [MenuItem("Tools/Addressables/Register All Sprites")]
    public static void RegisterAllSpritesInHeroesFolder()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup("PlayerAnimation");

        string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite");

        foreach (string guid in spriteGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = System.IO.Path.GetFileNameWithoutExtension(path); // 예: cloth1_c1_bot
        }

        Debug.Log($"스프라이트 {spriteGuids.Length}개 자동 등록 완료!");
    }
}
#endif
