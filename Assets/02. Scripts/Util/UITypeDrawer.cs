#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// UIType enum 전용 Drawer 등록
[CustomPropertyDrawer(typeof(UIType))]
public class UITypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // BeginProperty / EndProperty 는 Undo/Prefab 등 처리에 필요합니다.
        EditorGUI.BeginProperty(position, label, property);

        // int 값 → enum 으로 변환
        UIType current = (UIType)property.intValue;
        // Unity 내장 FlagsField 로 그려 주면 MaskFieldDropDown 버그를 피할 수 있습니다.
        UIType next = (UIType)EditorGUI.EnumFlagsField(position, label, current);

        // 선택된 enum 값을 다시 int 에 저장
        property.intValue = (int)next;

        EditorGUI.EndProperty();
    }
}
#endif
