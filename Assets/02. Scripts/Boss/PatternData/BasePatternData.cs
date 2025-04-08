using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BasePatternData : ScriptableObject
{
    protected Transform boss;

    [SerializeField] protected Transform target;

    [SerializeField] protected int layerMask = LayerMask.GetMask("Player");
    
    [SerializeField] public List<Rect> attackableAreas;

    [SerializeField] public Color gizmoColor = new Color(1, 0, 0, 0.3f);
    
    public void Init(Transform boss)
    {
        this.boss = boss;
    }

    /// <summary>
    /// 패턴이 사용가능한지 여부를 판단(감지영역 내 플레이어 존재하는가)
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable()
    {
        for (int i = 0; i < attackableAreas.Count; i++)
        {
            
            Vector2 pointA = new Vector2(attackableAreas[i].xMin, attackableAreas[i].yMin);
            Vector2 pointB = new Vector2(attackableAreas[i].xMax, attackableAreas[i].yMax);
            pointA = boss.TransformPoint(pointA);
            pointB = boss.TransformPoint(pointB);
            Collider2D hit = Physics2D.OverlapArea(pointA, pointB, layerMask);

            if (hit != null)
            {
                target = hit.transform;
                return true;
            }
        }
        return false;
    }
    public abstract IEnumerator ExecutePattern(Transform bossTransform,Animator animator);
}
