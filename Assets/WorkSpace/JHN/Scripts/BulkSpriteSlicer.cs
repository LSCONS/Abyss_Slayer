using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class BulkSpriteSlicer
{
    [MenuItem("Tools/Slice All PNGs in Folder")]
    public static void SliceAllPNGsInFolder()
    {
        // 1) 애니메이션별 프레임 수 정의
        var animationMap = new List<(string name, int count)>
        {
            ("Idle1",   6),
            ("Idle2",   6),
            ("Run1",    8),
            ("Run2",    8),
            ("Jump",    8),
            ("Attack1", 6),
            ("Attack2", 6),
            ("Attack3", 4),
            ("AirAtk1", 6),
            ("AirAtk2", 4),
            ("Casting1",5),
            ("Casting2",5),
            ("Hurt",    4),
            ("Dying",   5),
            ("Dash",    8),
            ("Block",   5),
            ("Roll",    8),
        };

        // 2) 폴더 선택
        string folderPath = EditorUtility.OpenFolderPanel(
            "Select Folder with PNGs",
            Application.dataPath,
            ""
        );
        if (string.IsNullOrEmpty(folderPath)) return;

        // 3) 모든 PNG 파일 순회
        string[] pngFiles = Directory.GetFiles(
            folderPath,
            "*.png",
            SearchOption.AllDirectories
        );

        foreach (string file in pngFiles)
        {
            string assetPath = "Assets" + file
                .Replace(Application.dataPath, "")
                .Replace("\\", "/");

            // 4) Importer 가져오기
            TextureImporter importer = AssetImporter
                .GetAtPath(assetPath) as TextureImporter;
            if (importer == null) continue;

            // 5) 스프라이트 설정
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            // meshType = FullRect 로 변경
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);

            importer.isReadable = true;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // 6) 텍스처 로드
            Texture2D texture = AssetDatabase
                .LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                Debug.LogWarning($"Failed to load texture: {assetPath}");
                continue;
            }

            // 7) 슬라이스 크기
            int sliceWidth = 80;
            int sliceHeight = 40;
            int paddingX = 20;

            // 8) 메타데이터 리스트 생성
            var metasList = new List<SpriteMetaData>();

            for (int row = 0; row < animationMap.Count; row++)
            {
                var (animName, frameCount) = animationMap[row];
                float y = texture.height - sliceHeight * (row + 1);

                for (int col = 0; col < frameCount; col++)
                {
                    // X 위치 계산: 프레임 너비 + 패딩을 곱해준다
                    float xPos = col * (sliceWidth + paddingX);

                    var meta = new SpriteMetaData
                    {
                        rect = new Rect(
                            xPos,      // 패딩 포함된 x 좌표
                            y,
                            sliceWidth,
                            sliceHeight
                        ),
                        name = $"{Path.GetFileNameWithoutExtension(assetPath)}_{metasList.Count}",
                        alignment = (int)SpriteAlignment.Center
                    };
                    metasList.Add(meta);
                }
            }

            // 9) 적용 & 리임포트
            importer.spritesheet = metasList.ToArray();
            importer.SaveAndReimport();

            Debug.Log(
                $"Sliced {assetPath} into {metasList.Count} sprites " +
                "(defined by animationMap)."
            );
        }

        AssetDatabase.Refresh();
        Debug.Log("모든 PNG 이미지에 자동 Slice 완료.");
    }
}
