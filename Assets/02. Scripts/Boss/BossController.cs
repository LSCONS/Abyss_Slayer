using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UI.Image;

[Serializable]
public class BossPattern
{
    [Header("패턴 정보")]
    [Tooltip("가중치")]
    public float weight;
    public BasePatternData patternData;

    [Header("테스트용 항목")]
    [Tooltip("끄면 패턴 비활성화")]
    public bool setActivePatternForTest;
    public bool showGizmos;
   
    
}
public class BossController : MonoBehaviour
{
    [SerializeField] BasePatternData appearPattern;
    [SerializeField] List<BossPattern> allPatterns;
    [SerializeField] GameObject targetCrossHairPrefab;
    Animator animator;
    Transform _targetCrosshair;
    GameObject _targetCrosshairObj;
    Transform _target;
    [SerializeField] public SpriteRenderer sprite;      //보스스프라이트
    public Collider2D hitCollider;                      //보스 피격판정 콜라이더
    [SerializeField] SpriteRenderer _effectSprite;
    [SerializeField] public float bossCenterHight;
    public float mapWidth;
    public CinemachineVirtualCamera virtualCamera;

    [HideInInspector] public bool chasingTarget;
    bool _showTargetCrosshair;
    public bool showTargetCrosshair
    {
        get { return _showTargetCrosshair; }
        set 
        {
            if (value && _target != null) _targetCrosshair.position = _target.position;
            _targetCrosshairObj.SetActive(value);
            _showTargetCrosshair = value;
        }
    }
    
    bool _isLeft;
    public bool isLeft
    {
        get { return _isLeft; }
        set 
        {
            if(_isLeft != value)
            {
                _isLeft = value;
                sprite.flipX = value;
            }
        }
    }
    bool _isRun;
    public bool isRun
    {
        get { return _isRun; }
        set
        {
            if (_isRun != value)
            {
                _isRun = value;
                animator.SetBool("Run",value);
            }
        }
    }

    [Header("움직임 관련")]
    [SerializeField] float runSpeed;
    [SerializeField] float jumpMoveTime;
    [SerializeField] float jumpMoveHight;
    [SerializeField] float gravityMultiplier;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitCollider = GetComponent<Collider2D>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        for (int i = 0; i < allPatterns.Count; i++)     //소지한 모든 패턴데이터에 자신의 정보 삽입
        {
            allPatterns[i].patternData.Init(transform,this,animator);
        }
        appearPattern?.Init(transform, this, animator);

        _targetCrosshair = Instantiate(targetCrossHairPrefab).transform;
        _targetCrosshairObj = _targetCrosshair.gameObject;

        chasingTarget = false;
        showTargetCrosshair = false;
        _isRun = false;
    }

    private void Start()
    {
        StartCoroutine(startLoop());
    }

    private void Update()
    {
        if (chasingTarget)
        {
            isLeft = (_target.position.x - transform.position.x < 0);
        }
        if (showTargetCrosshair)
        {
            _targetCrosshair.position = _target.position;
        }
        
    }
    IEnumerator startLoop()
    {
        if(appearPattern != null)
        {
            yield return appearPattern.ExecutePattern();
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
    IEnumerator PatternLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Landing());
            BossPattern next = GetRandomPattern();
            if (next != null)
            {
                _target = next.patternData.target;
                yield return StartCoroutine(next.patternData.ExecutePattern());
            }
            else
            {
                //Debug.LogWarning("선택 가능한 패턴 없음. 대기");
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 각 패턴이 가지는 감지영역 시각화, 상대적 위치영역, 절대적 위치영역 2종류 존재
    /// </summary>
    private void OnDrawGizmos()
    {
        if(allPatterns.Count == 0)
            return;
        for(int i = 0; i < allPatterns.Count; i++)
        {
            BossPattern pattern = allPatterns[i];

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
        StartCoroutine(GetRandomPattern().patternData.ExecutePattern());
    }
    BossPattern GetRandomPattern()
    {
        // 가능한 패턴만 필터링, 테스트용 활성화 값이 참인경우만 포함
        List<BossPattern> availablePatterns = allPatterns.Where(p =>p.setActivePatternForTest && p.patternData.IsAvailable()).ToList();

        if (availablePatterns.Count == 0)
            return null;

        // 가중치 총합 계산
        float totalWeight = availablePatterns.Sum(p => p.weight);

        // 랜덤 값 생성
        float selectedValue = UnityEngine.Random.Range(0f, totalWeight);

        // 해당 구간의 패턴 선택
        float cumulative = 0f;
        foreach (var pattern in availablePatterns)
        {
            cumulative += pattern.weight;
            if (selectedValue <= cumulative)
            {
                return pattern;
            }
        }
        return availablePatterns.Last(); // 예외 방지용 (총합이 float 연산으로 어긋날 경우, 없으면 반환없는경우 생겨서 에러남)
    }

    [SerializeField] LayerMask _groundLayerMask;
    public IEnumerator Landing(bool isDeath = false)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f,_groundLayerMask);

        if (hit.point.y > transform.position.y - bossCenterHight + 0.05f)
        {
            while (hit.point.y >= transform.position.y - bossCenterHight + 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hit.point + (Vector2.up * bossCenterHight), 8f * Time.deltaTime);
                yield return null;
            }
            transform.position = hit.point + (Vector2.up * bossCenterHight);
        }
        else if (hit.point.y < transform.position.y - bossCenterHight - 0.05f)
        {
            animator.SetTrigger(isDeath? "DeathFall" : "Fall");
            float time = 0f;
            float startHight = transform.position.y;
            while (hit.point.y <= transform.position.y - bossCenterHight - 0.05f)
            {
                transform.position = new Vector3(transform.position.x, startHight - (0.5f * 9.8f * gravityMultiplier * time * time));
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = hit.point + (Vector2.up * bossCenterHight);

            if (!isDeath)
            {
                animator.SetTrigger("Land");
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
        Vector3 startPosition = transform.position;
        float _jumpMoveTime = (inputJumpMoveTime <= 0)? jumpMoveTime : inputJumpMoveTime;
        float _jumpMoveHight = (inputJumpMoveHight <= -10)? jumpMoveHight : inputJumpMoveHight;
        float maxY = Mathf.Max(targetPosition.y, startPosition.y) + _jumpMoveHight;
        float deltaY1 = maxY - startPosition.y;
        float deltaY2 = maxY - targetPosition.y;
        float hightestTime = _jumpMoveTime / (1 + Mathf.Sqrt(deltaY2 / deltaY1));
        float jumpGravity = 2 * deltaY1 / (hightestTime * hightestTime);
        float startVelocityY = jumpGravity * hightestTime;

        isLeft = targetPosition.x - transform.position.x <= 0;
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.2f);
        PoolManager.Instance.Get<JumpEffect>().Init(transform.position + Vector3.down * bossCenterHight);
        float time = 0f;
        while(time < _jumpMoveTime)
        {
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, time / _jumpMoveTime);
            if(time >= hightestTime)
                animator.SetTrigger("Fall");
            float y = startPosition.y + (startVelocityY * time) - (0.5f * jumpGravity * time * time);
            transform.position = new Vector3(x, y, 0);
            time += Time.deltaTime;
            yield return null;
        }
        animator.ResetTrigger("Fall");
        animator.SetTrigger("Land");
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.4f);
    }
    public IEnumerator RunMove(bool isleft, float speed = -1f)
    {
        bool isfall = false;
        float startHight = transform.position.y;
        float time = 0f;

        float _speed;
        if (speed <= 0f) _speed = isleft ? -runSpeed : runSpeed;
        else _speed = isleft ? -speed : speed;
        isLeft = isleft;
        isRun = true;
        
        while (isRun)
        {
            float x = Mathf.Clamp(transform.position.x + _speed * Time.deltaTime, -mapWidth / 2 + 0.7f, mapWidth / 2 - 0.7f);
            transform.position = new Vector3 (x, transform.position.y, 0);

            if (!Physics2D.Raycast(transform.position, Vector3.down, bossCenterHight + 0.01f, LayerMask.GetMask("GroundPlane", "GroundPlatform")))
            {
                if (!isfall)
                {
                    startHight = transform.position.y;
                    time = 0f;
                    isfall = true;
                }
                transform.position = new Vector3(transform.position.x, startHight - (0.5f * 9.8f * gravityMultiplier * time * time));
                time += Time.deltaTime;
            }
            else
            {
                if (isfall)
                {
                    isfall = false;
                }
            }
            yield return null;
        }
    }

    public void OnDead()
    {
        StopAllCoroutines();
        StartCoroutine(Dead());
    }

    IEnumerator Dead()
    {
        //yield return StartCoroutine(Landing(true));
        Time.timeScale = 0.2f;
        animator.SetTrigger("Dead");
        virtualCamera.Priority = 20;
        yield return new WaitForSeconds(0.2f);

        while (Time.timeScale < 1f)
        {
            Time.timeScale += 0.1f;
            yield return null;
        }
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2f);
        virtualCamera.Priority = 5;
    }
}
