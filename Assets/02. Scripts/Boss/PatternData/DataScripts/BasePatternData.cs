using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BasePatternData : ScriptableObject
{
    protected Transform bossTransform;
    protected BossController bossController;
    protected Animator bossAnimator;
    protected Boss boss;
    protected float bossCenterHight;
    protected float mapWidth;
    protected PlayerRef playerRef => target.GetComponent<Player>().PlayerRef;

    [Header("패턴 공통 정보")]
    public Transform target;

    [SerializeField] public List<Rect> attackableAreas;
    [SerializeField] public List<Rect> globalAttackableAreas;
    [SerializeField] public Color gizmoColor = new Color(1, 0, 0, 0.3f);

    public void Init(BossController controller)
    {
        this.bossTransform = controller.transform;
        this.bossController = controller;
        boss = controller.Boss;
        bossAnimator = controller.Animator;
        bossCenterHight = controller.BossCenterHight;
        mapWidth = controller.MapWidth;
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
            pointA = bossTransform.TransformPoint(pointA);
            pointB = bossTransform.TransformPoint(pointB);

            Collider2D hit = Physics2D.OverlapArea(pointA, pointB, LayerData.PlayerLayerMask);

            if (hit != null)
            {
                target = hit.transform;
                return true;
            }
        }
        for (int i = 0; i < globalAttackableAreas.Count; i++)
        {

            Vector2 pointA = new Vector2(globalAttackableAreas[i].xMin, globalAttackableAreas[i].yMin);
            Vector2 pointB = new Vector2(globalAttackableAreas[i].xMax, globalAttackableAreas[i].yMax);


            Collider2D hit = Physics2D.OverlapArea(pointA, pointB, LayerData.PlayerLayerMask);

            if (hit != null)
            {
                target = hit.transform;
                return true;
            }
        }
        return false;
    }
    public abstract IEnumerator ExecutePattern();
}
