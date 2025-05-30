#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AnimationEventRenamer : EditorWindow
{
    string oldName = "";
    string newName = "";
    bool onlySelectedClips = true;

    [MenuItem("Tools/Animation Event Renamer")]
    static void OpenWindow()
    {
        GetWindow<AnimationEventRenamer>("Event Renamer");
    }

    void OnGUI()
    {
        GUILayout.Label("AnimationEvent Function Name Replacer", EditorStyles.boldLabel);
        oldName = EditorGUILayout.TextField("Old Name", oldName);
        newName = EditorGUILayout.TextField("New Name", newName);
        onlySelectedClips = EditorGUILayout.Toggle("Only Selected Clips", onlySelectedClips);

        if (GUILayout.Button("Replace"))
        {
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName))
            {
                Debug.LogWarning("Old Name/New Name 둘 다 입력해야 합니다.");
            }
            else
            {
                ReplaceEventNames();
            }
        }
    }

    void ReplaceEventNames()
    {
        // 1) 대상 클립 찾기
        string[] guids;
        if (onlySelectedClips && Selection.objects.Length > 0)
        {
            // 선택된 애니메이션 에셋만
            guids = AssetDatabase.FindAssets("t:AnimationClip", Selection.assetGUIDs);
        }
        else
        {
            // 프로젝트 내 모든 AnimationClip
            guids = AssetDatabase.FindAssets("t:AnimationClip");
        }

        int totalChanged = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;

            // 2) 이벤트 가져오기
            var events = AnimationUtility.GetAnimationEvents(clip);
            bool dirty = false;

            // 3) 함수 이름 교체
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].functionName == oldName)
                {
                    events[i].functionName = newName;
                    dirty = true;
                }
            }

            // 4) 교체가 일어났다면 다시 저장
            if (dirty)
            {
                AnimationUtility.SetAnimationEvents(clip, events);
                EditorUtility.SetDirty(clip);
                totalChanged++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"AnimationEventRenamer: {totalChanged}개 클립에서 \"{oldName}\" → \"{newName}\" 교체 완료.");
    }
}
#endif
