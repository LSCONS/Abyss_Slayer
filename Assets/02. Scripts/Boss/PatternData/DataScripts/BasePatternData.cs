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
    public virtual bool IgnoreAvailabilityCheck => false;   // 범위 체크해서 스킬 사용할건지

    [field: Header("패턴 사운드 설정")]
    [field: SerializeField] public List<EAudioClip> EAudioClip { get; private set; } = new();

    public void Init(BossController controller)
    {
#if AllMethodDebug
        Debug.Log("Init");
#endif
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
#if AllMethodDebug
        Debug.Log("IsAvailable");
#endif
        for (int i = 0; i < attackableAreas.Count; i++)
        {
            Vector2 pointA = new Vector2(attackableAreas[i].xMin, attackableAreas[i].yMin);
            Vector2 pointB = new Vector2(attackableAreas[i].xMax, attackableAreas[i].yMax);
            pointA = bossTransform.TransformPoint(pointA);
            pointB = bossTransform.TransformPoint(pointB);

            List<Player> players = SerchPlayerInArea(pointA, pointB);

            if (players.Count != 0)
            {
                target = players[0].transform;
                return true;
            }
        }

        for (int i = 0; i < globalAttackableAreas.Count; i++)
        {

            Vector2 pointA = new Vector2(globalAttackableAreas[i].xMin, globalAttackableAreas[i].yMin);
            Vector2 pointB = new Vector2(globalAttackableAreas[i].xMax, globalAttackableAreas[i].yMax);


            List<Player> players = SerchPlayerInArea(pointA, pointB);

            if (players.Count != 0)
            {
                target = players[0].transform;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 특정 사각형 범위 내에 플레이어가 있는지 검사하고 가장 가까운 플레이어부터 0번으로 정렬해서 List<Player>를 반환하는 메서드
    /// </summary>
    /// <param name="minXY"></param>
    /// <param name="maxXY"></param>
    /// <returns></returns>
    private List<Player> SerchPlayerInArea(Vector2 minXY, Vector2 maxXY)
    {
        List<Player> playerList = new();

        foreach(Player player in ServerManager.Instance.DictRefToPlayer.Values)
        {
            Vector2 playerVector2 = player.transform.position;
            if (playerVector2.x < minXY.x || playerVector2.y < minXY.y) continue;
            if (playerVector2.x > maxXY.x || playerVector2.y > maxXY.y) continue;
            playerList.Add(player);
        }

        playerList.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(a.transform.position, boss.transform.position);
            float distanceB = Vector2.Distance(b.transform.position, boss.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return playerList;
    }


    public abstract IEnumerator ExecutePattern();
}
