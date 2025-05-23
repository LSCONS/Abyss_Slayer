using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Fusion;
using UnityEngine;

[Serializable]
public class BossPattern
{
    [Header("패턴 정보"), Tooltip("가중치: 높을수록 높은 확률로 선택됨.")]
    public float weight = 1;
    [Tooltip("넣을 패턴 데이터")]
    public BasePatternData patternData;
    [Tooltip("같은 패턴을 사용하기 위한 딜레이 시간. 0이면 매번 패턴 데이터에 참조")]
    public float samePatternDelayTime = 0;
    public float SamePatternDelayCurTime { get; set; } = 0;

    [Header("테스트용 항목"), Tooltip("패턴 활성화 여부")]
    public bool setActivePatternForTest = true;
    [Tooltip("Debug Scene에 적용 범위를 보여주고 싶다면 활성화")]
    public bool showGizmos = true;

    [Header("패턴 적용 조건 정보"), Tooltip("패턴 활성화 조건 여부. 보스 체력이 더 많을경우 비활성화")]
    public bool isCheckActiveTrue = false;
    [Tooltip("패턴 비활성화 조건 여부. 보스 체력이 더 적을경우 비활성화")]
    public bool isCheckActiveFalse= false;
    [Tooltip("패턴을 적용할 보스 체력 비율(0 ~ 1)"), Range(0, 1)]
    public float bossCheckHP = 0.5f;
}


public class BossController : NetworkBehaviour
{
    [field: SerializeField] private BasePatternData     AppearPattern           { get; set; }
    [field: SerializeField] private BasePatternData     Phase2Pattern           { get; set; }
    [field: SerializeField] private List<BossPattern>   AllPatterns             { get; set; }
    [field: SerializeField] public float                MapWidth                { get; set; }
    [field: SerializeField] public float                BossCenterHight         { get; private set; }
    [field: SerializeField] public Boss                 Boss                    { get; private set; }
    public Transform                                    Target                  { get; private set; }
    public bool                                         ChasingTarget           { get; set; }
    public CinemachineVirtualCamera                     VirtualCamera           => Boss.VirtualCamera;
    public Collider2D                                   HitCollider             => Boss.HitCollider; //보스 피격판정 콜라이더
    public Animator                                     Animator                => Boss.Animator;
    public SpriteRenderer                               Sprite                  => Boss.Sprite; //보스스프라이트


    private bool _showTargetCrosshair;
    public bool ShowTargetCrosshair
    {
        get { return _showTargetCrosshair; }
        set 
        {
            PoolManager.Instance.CrossHairObject.gameObject.SetActive(value);
            _showTargetCrosshair = value;
        }
    }


    bool _isRun;
    public bool IsRun
    {
        get { return _isRun; }
        set
        {
            if (_isRun != value)
            {
                _isRun = value;
                Boss.Rpc_SetBoolAnimationHash(AnimationHash.RunParameterHash ,value);
            }
        }
    }

    [field: Header("움직임 관련")]
    [field: SerializeField] private float JumpMoveTime      { get; set; }
    [field: SerializeField] private float JumpMoveHight     { get; set; }
    [field: SerializeField] private float GravityMultiplier { get; set; }
    [field: SerializeField] private float runSpeed { get; set; }

    // 페이즈 관련
    private bool hasEnteredPhase2 = false;      // 2페 들갔나 확인

    public void Init()
    {
        for (int i = 0; i < AllPatterns.Count; i++)     //소지한 모든 패턴데이터에 자신의 정보 삽입
        {
            AllPatterns[i].patternData.Init(this);
        }
        AppearPattern?.Init(this);
        Phase2Pattern?.Init(this);

        if (Runner.IsServer || PoolManager.Instance.CrossHairObject == null)
        {
            Runner.Spawn(DataManager.Instance.CrossHairPrefab);
        }

        _isRun = false;
        ChasingTarget = false;
        ShowTargetCrosshair = false;
    }

    public void StartBossPattern()
    {
#if AllMethodDebug
        Debug.Log("StartBossPattern");
#endif
        if(Boss.IsRest && RunnerManager.Instance.GetRunner().IsServer)
        StartCoroutine(startLoop());
    }


    IEnumerator startLoop()
    {
#if AllMethodDebug
        Debug.Log("startLoop");
#endif
        if (AppearPattern != null)
        {
            yield return AppearPattern.ExecutePattern();
        }
        else
        {
            Debug.Log("시작 연출 패턴이 null입니다");
        }
        StartCoroutine(PatternLoop());
    }

    /// <summary>
    /// 보스패턴 지속적 호출 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator PatternLoop()
    {
#if AllMethodDebug
        Debug.Log("PatternLoop");
#endif
        while (true)
        {
            yield return StartCoroutine(Landing());

            // 1. 2페이즈 진입 체크
            if (!hasEnteredPhase2 && (float)Boss.Hp.Value / Boss.MaxHp.Value <= 0.5f)
            {
                hasEnteredPhase2 = true; // 절대 먼저 실행되도록

                yield return StartCoroutine(PlayPhase2());
                continue;
            }

            // 2. 일반 패턴 루프
            BossPattern next = GetRandomPattern();
            if (next != null)
            {
                if (next.patternData.target != null)
                    Target = next.patternData.target;
                else if (Target == null)
                    Target = GetClosestPlayerTransform();

                Target = next.patternData.target;
                yield return StartCoroutine(next.patternData.ExecutePattern());
                if (next.samePatternDelayTime > 0)
                    StartCoroutine(CheckDelayCompute(next));
            }
            else
            {
                Debug.Log("선택 가능한 패턴 없음. 대기");
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 2페이즈 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayPhase2()
    {
        if (Phase2Pattern == null)
        {
            Debug.LogWarning("2페 데이터가 없습니다");
            yield break;
        }

        Debug.Log("2페 들어가기");
        yield return StartCoroutine(Phase2Pattern.ExecutePattern());
    }

    /// <summary>
    /// 각 패턴이 가지는 감지영역 시각화, 상대적 위치영역, 절대적 위치영역 2종류 존재
    /// </summary>
    private void OnDrawGizmos()
    {
        if(AllPatterns.Count == 0)
            return;
        for(int i = 0; i < AllPatterns.Count; i++)
        {
            BossPattern pattern = AllPatterns[i];

            if (pattern == null || (pattern.patternData.attackableAreas.Count == 0 && pattern.patternData.globalAttackableAreas.Count == 0))
                return;
            if (!pattern.setActivePatternForTest || !pattern.showGizmos)
                continue;
            Gizmos.color = pattern.patternData.gizmoColor;

            for(int j = 0; j < pattern.patternData.attackableAreas.Count; j++)
            {
                Rect rect = pattern.patternData.attackableAreas[j]; // Rect는 2D 기준: position은 좌하단 기준, size는 width/height

                Vector3 center = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, transform.position.z);
                center = transform.TransformPoint(center);

                Vector3 size = new Vector3(rect.width, rect.height, 0f);

                Gizmos.DrawCube(center, size);
            }
            for (int j = 0; j < pattern.patternData.globalAttackableAreas.Count; j++)
            {
                Rect rect = pattern.patternData.globalAttackableAreas[j]; // Rect는 2D 기준: position은 좌하단 기준, size는 width/height

                Vector3 center = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, transform.position.z);

                Vector3 size = new Vector3(rect.width, rect.height, 0f);

                Gizmos.DrawCube(center, size);
            }
        }
    }

    public void StartNextPattern()
    {
#if AllMethodDebug
        Debug.Log("StartNextPattern");
#endif
        StartCoroutine(GetRandomPattern().patternData.ExecutePattern());
    }


    BossPattern GetRandomPattern()
    {
#if AllMethodDebug
        Debug.Log("GetRandomPattern");
#endif
        // 가능한 패턴만 필터링, 테스트용 활성화 값이 참인경우만 포함
        List<BossPattern> patterns = new();
        foreach(BossPattern pattern in AllPatterns)
        {
            //실행 비활성화일 경우 무시
            if (!(pattern.setActivePatternForTest)) continue;
            //딜레이 시간이 남아있는지 확인
            if (pattern.SamePatternDelayCurTime > 0) continue;
            //패턴 조건 체크 결과 보스 체력이 더 많을경우 비활성화
            if (pattern.isCheckActiveTrue && ((float)Boss.Hp.Value / Boss.MaxHp.Value) >= pattern.bossCheckHP) continue;
            //패턴 조건 체크 결과 보스 체력이 더 적을경우 비활성화
            if (pattern.isCheckActiveFalse && ((float)Boss.Hp.Value /Boss.MaxHp.Value) < pattern.bossCheckHP) continue;
            //패턴 안에 실행 가능한 플레이어가 없을 경우 무시
            if (!pattern.patternData.IgnoreAvailabilityCheck && !pattern.patternData.IsAvailable()) continue;

            patterns.Add(pattern);
        }
        Debug.Log($"AllpatternCount = {AllPatterns.Count}");
        Debug.Log($"listCount = {patterns.Count}");

        if (patterns.Count == 0)
            return null;

        // 가중치 총합 계산
        float totalWeight = patterns.Sum(p => p.weight);

        // 랜덤 값 생성
        float selectedValue = UnityEngine.Random.Range(0f, totalWeight);

        // 해당 구간의 패턴 선택
        float cumulative = 0f;
        foreach (var pattern in patterns)
        {
            cumulative += pattern.weight;
            if (selectedValue <= cumulative)
            {
                return pattern;
            }
        }
        return patterns.Last(); // 예외 방지용 (총합이 float 연산으로 어긋날 경우, 없으면 반환없는경우 생겨서 에러남)
    }

    private IEnumerator CheckDelayCompute(BossPattern pattern)
    {
        pattern.SamePatternDelayCurTime = pattern.samePatternDelayTime;
        while(true)
        {
            yield return null;
            pattern.SamePatternDelayCurTime -= Time.deltaTime;
            if(pattern.SamePatternDelayCurTime < 0f)
            {
                pattern.SamePatternDelayCurTime = 0f;
                break;
            }
        }
    }


    [SerializeField] LayerMask _groundLayerMask;
    public IEnumerator Landing(bool isDeath = false)
    {
#if AllMethodDebug
        Debug.Log("Landing");
#endif
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        RaycastHit2D hit = scene2D.Raycast(transform.position, Vector2.down, 10f,_groundLayerMask);

        if (hit.point.y > transform.position.y - BossCenterHight + 0.05f)
        {
            while (hit.point.y >= transform.position.y - BossCenterHight + 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hit.point + (Vector2.up * BossCenterHight), 8f * Time.deltaTime);
                yield return null;
            }
            transform.position = hit.point + (Vector2.up * BossCenterHight);
        }
        else if (hit.point.y < transform.position.y - BossCenterHight - 0.05f)
        {
            int hash = isDeath ? AnimationHash.DeadParameterHash : AnimationHash.FallParameterHash;
            Boss.Rpc_SetTriggerAnimationHash(hash);

            float time = 0f;
            float startHight = transform.position.y;
            while (hit.point.y <= transform.position.y - BossCenterHight - 0.05f)
            {
                transform.position = new Vector3(transform.position.x, startHight - (0.5f * 9.8f * GravityMultiplier * time * time));
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = hit.point + (Vector2.up * BossCenterHight);

            if (!isDeath)
            {
                Boss.Rpc_SetTriggerAnimationHash(AnimationHash.LandParameterHash);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    /// <summary>
    /// 현재 좌표에서 지정장소까지 점프해서 이동하는 코루틴
    /// </summary>
    /// <param name="targetPosition">이동할(도착)장소</param>
    /// <param name="inputJumpMoveTime">도착까지 걸리는 시간 (음수입력시 기본값)</param>
    /// <param name="inputJumpMoveHight">점프높이(더 높은쪽에 +되는 높이, 음수입력시 기본값)</param>
    /// <returns></returns>
    public IEnumerator JumpMove(Vector3 targetPosition, float inputJumpMoveTime = -1f, float inputJumpMoveHight = -10f)
    {
#if AllMethodDebug
        Debug.Log("JumpMove");
#endif
        Vector3 startPosition = transform.position;
        float _jumpMoveTime = (inputJumpMoveTime <= 0)? JumpMoveTime : inputJumpMoveTime;
        float _jumpMoveHight = (inputJumpMoveHight < 0)? JumpMoveHight : inputJumpMoveHight;
        float maxY = Mathf.Max(targetPosition.y, startPosition.y) + _jumpMoveHight;
        float deltaY1 = maxY - startPosition.y;
        float deltaY2 = maxY - targetPosition.y;
        float hightestTime = _jumpMoveTime / (1 + Mathf.Sqrt(deltaY2 / deltaY1));
        float jumpGravity = 2 * deltaY1 / (hightestTime * hightestTime);
        float startVelocityY = jumpGravity * hightestTime;

        Boss.IsLeft = targetPosition.x - transform.position.x <= 0;
        Boss.Rpc_SetTriggerAnimationHash(AnimationHash.JumpParameterHash);
        yield return new WaitForSeconds(0.2f);
        PoolManager.Instance.Get<JumpEffect>().Rpc_Init(transform.position + Vector3.down * BossCenterHight);
        float time = 0f;
        while (time < _jumpMoveTime)
        {
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, time / _jumpMoveTime);
            if (time >= hightestTime)
                Animator .SetTrigger(AnimationHash.FallParameterHash);
            float y = startPosition.y + (startVelocityY * time) - (0.5f * jumpGravity * time * time);
            transform.position = new Vector3(x, y, 0);
            time += Time.deltaTime;
            yield return null;
        }
        Boss.Rpc_ResetTriggerAnimationHash(AnimationHash.FallParameterHash);
        if (_jumpMoveHight != 0)
            Boss.Rpc_SetTriggerAnimationHash(AnimationHash.LandParameterHash);
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.4f);
    }
    public IEnumerator RunMove(bool isleft, float speed = -1f)
    {
#if AllMethodDebug
        Debug.Log("RunMove");
#endif
        
        bool isfall = false;
        float startHight = transform.position.y;
        float time = 0f;

        float _speed;
        if (speed <= 0f) _speed = isleft ? -runSpeed : runSpeed;
        else _speed = isleft ? -speed : speed;
        Boss.IsLeft = isleft;
        IsRun = true;
        
        while (IsRun)
        {
            float x = Mathf.Clamp(transform.position.x + _speed * Time.deltaTime, -MapWidth / 2 + 0.7f, MapWidth / 2 - 0.7f);
            transform.position = new Vector3 (x, transform.position.y, 0);

            if (!IsLand())
            {
                if (!isfall)
                {
                    startHight = transform.position.y;
                    time = 0f;
                    isfall = true;
                }
                transform.position = new Vector3(transform.position.x, startHight - (0.5f * 9.8f * GravityMultiplier * time * time));
                time += Time.deltaTime;
            }
            else
            {
                if (isfall)
                {
                    PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
                    float posY = scene2D.Raycast(transform.position, Vector3.down, 20, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask).point.y + BossCenterHight;
                    transform.position = new Vector3(transform.position.x, posY);
                    isfall = false;
                }
            }
            yield return null;
        }
    }
    public bool IsLand()
    {
        PhysicsScene2D scene2D = RunnerManager.Instance.GetRunner().GetPhysicsScene2D();
        return scene2D.Raycast(transform.position, Vector3.down, BossCenterHight + 0.01f, LayerData.GroundPlaneLayerMask | LayerData.GroundPlatformLayerMask);
    }

    /// <summary>
    /// 몬스터의 체력이 전부 닳았을 때 실행할 메서드
    /// </summary>
    public void OnDead()
    {
#if AllMethodDebug
        Debug.Log("OnDead");
#endif
        StopAllCoroutines();
        StartCoroutine(Dead());
    }


    /// <summary>
    /// 몬스터의 체력이 전부 닳았을 때 실행할 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator Dead()
    {
#if AllMethodDebug
        Debug.Log("Dead");
#endif
        yield return StartCoroutine(Landing(true));
        Time.timeScale = 0.2f;
        Boss.Rpc_SetTriggerAnimationHash(AnimationHash.DeadParameterHash);
        VirtualCamera.Priority = 20;
        yield return new WaitForSeconds(0.2f);

        while (Time.timeScale < 1f)
        {
            Time.timeScale += 0.1f;
            yield return null;
        }
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2f);
        VirtualCamera.Priority = 5;
    }

    private Transform GetClosestPlayerTransform()
    {
        Player closestPlayer = null;
        float minDistance = float.MaxValue;

        foreach (var player in ServerManager.Instance.DictRefToPlayer.Values)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPlayer = player;
            }
        }

        return closestPlayer?.transform;
    }
}
