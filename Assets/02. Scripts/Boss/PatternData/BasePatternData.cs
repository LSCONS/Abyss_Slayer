using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern", menuName = "BossPattern")]
public class BasePatternData : ScriptableObject
{
    [SerializeField] public List<Rect> attackableAreas;

    [SerializeField] public Color gizmoColor = new Color(1, 0, 0, 0.3f);

    private void OnDrawGizmosSelected()
    {
        if (attackableAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in attackableAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new Vector3(area.width, area.height);
            Gizmos.DrawCube(center, size);
        }
    }
}
