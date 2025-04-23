using UnityEngine;

public class MageSkillRangeVisualizer : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private Color gizmoColor = Color.yellow;

    public void SetRange(float newRange)
    {
        range = newRange;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && range > 0)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
} 