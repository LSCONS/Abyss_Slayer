#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class RecursiveSpriteImporter : EditorWindow
{
    // 기본 경로 (프로젝트 뷰상의 상대 경로)
    private string targetFolder = "Assets/99. Assets/UI_test/PNG";

    [MenuItem("Tools/Sprite Batch Import (Recursive)")]
    public static void ShowWindow()
    {
        GetWindow<RecursiveSpriteImporter>("Sprite Batch Import");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite Import Settings Batch Processor (Recursive)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 폴더 경로 입력 + 브라우즈
        EditorGUILayout.BeginHorizontal();
        targetFolder = EditorGUILayout.TextField("Folder Path", targetFolder);
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(70)))
        {
            string abs = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(abs))
            {
                if (abs.StartsWith(Application.dataPath))
                    targetFolder = "Assets" + abs.Substring(Application.dataPath.Length);
                else
                    Debug.LogWarning("프로젝트 Assets 폴더 내부 경로만 지원됩니다.");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Settings to All PNGs Recursively"))
            ApplySettingsRecursive(targetFolder);
    }

    private static void ApplySettingsRecursive(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("[RecursiveSpriteImporter] 처리할 폴더 경로를 지정하세요.");
            return;
        }

        // 절대경로로 변환
        string absFolder = Path.GetFullPath(folderPath);
        if (!Directory.Exists(absFolder))
        {
            Debug.LogError($"[RecursiveSpriteImporter] 폴더를 찾을 수 없습니다: {folderPath}");
            return;
        }

        // ALL_DIRECTORIES 옵션으로 재귀 탐색
        string[] files = Directory.GetFiles(absFolder, "*.png", SearchOption.AllDirectories);
        int count = 0;
        string dataPath = Application.dataPath.Replace("\\", "/");

        foreach (var absPath in files)
        {
            // 에셋 경로로 변환: "C:/.../Assets/XXX.png" → "Assets/XXX.png"
            string unityPath = absPath.Replace("\\", "/").Replace(dataPath, "Assets");
            var ti = AssetImporter.GetAtPath(unityPath) as TextureImporter;
            if (ti == null) continue;

            // --- 원하는 스프라이트 설정 ---
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.spritePixelsPerUnit = 32;
            ti.filterMode = FilterMode.Point;
            ti.spriteBorder = new Vector4(16, 16, 16, 16);
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            // ---------------------------------

            ti.SaveAndReimport();
            count++;
        }

        Debug.Log($"[RecursiveSpriteImporter] 완료: {count}개 PNG에 설정 적용됨.");
    }
}
#endif
