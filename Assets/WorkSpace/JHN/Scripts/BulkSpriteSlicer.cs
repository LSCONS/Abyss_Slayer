#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class BulkSpriteSlicer
{
    [MenuItem("Assets/Slice Sprites in Selected Folder", true)]
    private static bool Validate_SliceSpritesInSelectedFolder()
    {
        var obj = Selection.activeObject;
        if (obj == null) return false;
        var path = AssetDatabase.GetAssetPath(obj);
        return AssetDatabase.IsValidFolder(path);
    }

    [MenuItem("Assets/Slice Sprites in Selected Folder")]
    private static void SliceSpritesInSelectedFolder()
    {
        // 선택된 폴더의 에셋 경로(e.g. "Assets/MySprites")
        string assetFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
        // 시스템 경로(e.g. "C:/.../Project/Assets/MySprites")
        string fullFolder = Path.GetFullPath(assetFolder);

        // PNG들만 가져오기
        var pngFiles = Directory.GetFiles(fullFolder, "*.png", SearchOption.AllDirectories);
        if (pngFiles.Length == 0)
        {
            Debug.LogWarning($"[{assetFolder}] 폴더에 PNG가 없습니다.");
            return;
        }

        // 프레임 수 정의 (기존 animationMap 그대로 쓰시면 됩니다)
        var animationMap = new List<(string name, int count)>
        {
            ("Idle1",6),("Idle2",6),("Run1",8),("Run2",8),
            ("Jump",8),("Attack1",6),("Attack2",6),("Attack3",4),
            ("AirAtk1",6),("AirAtk2",4),("Casting1",5),("Casting2",5),
            ("Hurt",4),("Dying",5),("Dash",8),("Block",5),
            ("Roll",8),
        };

        foreach (var filePath in pngFiles)
        {
            // Assets/ 이하로 바꿔 주기
            string assetPath = "Assets" + filePath
                .Replace(Path.GetFullPath(Application.dataPath), "")
                .Replace("\\", "/");

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.isReadable = true;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // 메타 설정
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (tex == null) continue;

            int sliceW = 80, sliceH = 40, padX = 20;
            var metas = new List<SpriteMetaData>();

            for (int row = 0; row < animationMap.Count; row++)
            {
                var (anim, cnt) = animationMap[row];
                float y = tex.height - sliceH * (row + 1);

                for (int col = 0; col < cnt; col++)
                {
                    float x = col * (sliceW + padX);
                    metas.Add(new SpriteMetaData
                    {
                        rect = new Rect(x, y, sliceW, sliceH),
                        name = $"{Path.GetFileNameWithoutExtension(assetPath)}_{metas.Count}",
                        alignment = (int)SpriteAlignment.Center
                    });
                }
            }

            importer.spritesheet = metas.ToArray();
            importer.SaveAndReimport();
            Debug.Log($"[Sliced] {assetPath} → {metas.Count} sprites");
        }

        AssetDatabase.Refresh();
        Debug.Log($"슬라이스 완료: {assetFolder}");
        RegisterAssetsToAddressables("PlayerAnimation", pngFiles);
    }

    private static void RegisterAssetsToAddressables(string label, string[] assetFullPaths)
    {
        // 1) AddressableAssetSettings 불러오기
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings를 찾을 수 없습니다!");
            return;
        }

        // 2) "PlayerAnimation" 그룹 찾기 / 없으면 생성
        var group = settings.FindGroup(label);
        if (group == null)
        {
            group = settings.CreateGroup(
                label,
                false,    // readOnly
                false,    // virtual
                false,    // canRename
                new List<AddressableAssetGroupSchema> {
                    settings.DefaultGroup.Schemas.Find(s => s is BundledAssetGroupSchema)
                }
            );
        }

        // 3) String 테두리: Assets/... 형태로 변환
        foreach (var fullPath in assetFullPaths)
        {
            var assetPath = "Assets" + fullPath
                .Replace(Path.GetFullPath(Application.dataPath), "")
                .Replace("\\", "/");

            // 최종 리소스가 스프라이트 배열이므로, Addressables에 등록할 GUID는 메인 텍스처가 아닌
            // 해당 스프라이트 에셋 각각일 수 있지만, 여기서는 텍스처 에셋 단위로 등록합니다.
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                continue;

            // 4) 중복 엔트리 체크
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                // 새로 등록
                entry = settings.CreateOrMoveEntry(guid, group, false, false);
                entry.SetAddress(Path.GetFileNameWithoutExtension(assetPath));
            }
            else
            {
                // 기존 그룹이 다르면 옮기기
                if (entry.parentGroup != group)
                    settings.MoveEntry(entry, group);
            }

            // 5) 라벨 추가
            if (!entry.labels.Contains(label))
                entry.labels.Add(label);
        }

        // 6) 저장
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        AssetDatabase.SaveAssets();
        Debug.Log($"Addressables: '{label}' 그룹에 {assetFullPaths.Length}개 에셋 등록 완료.");
    }
}
#endif
