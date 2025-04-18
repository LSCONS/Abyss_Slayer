using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BasePatternData : ScriptableObject
{

    protected Transform bossTransform;
    protected BossController bossController;
    protected Animator bossAnimator;
    protected float bossCenterHight;

    [Header("패턴 공통 정보")]
    public Transform target;

    [SerializeField] public List<Rect> attackableAreas;
    [SerializeField] public List<Rect> globalAttackableAreas;
    [SerializeField] public Color gizmoColor = new Color(1, 0, 0, 0.3f);

    public void Init(Transform boss, BossController controller, Animator animator)
    {
        this.bossTransform = boss;
        this.bossController = controller;
        bossAnimator = animator;
        bossCenterHight = controller.bossCenterHight;
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

            //수정 송제우: 필드에서 Layer정의로 오류가 생겨서 바꿨습니다.
            //레이어 데이터를 정리해둔 클래스가 있으니 해당 스크립트의 LEADME 참고 바랍니다.
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

            //수정 송제우: 필드에서 Layer정의로 오류가 생겨서 바꿨습니다.
            //레이어 데이터를 정리해둔 클래스가 있으니 해당 스크립트의 LEADME 참고 바랍니다.
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
