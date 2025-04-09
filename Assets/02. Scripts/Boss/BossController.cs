using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

[Serializable]
public class BossPattern
{
    public float weight;
    public BasePatternData patternData;
    public bool showGizmos;
    
}
public class BossController : MonoBehaviour
{
    [SerializeField] List<BossPattern> allPatterns;

    Animator animator;
    [SerializeField] Transform targetCrosshair;
    [SerializeField] private LineRenderer targetLine;
    Transform target;

    public bool showTargetCrosshair;
    bool preShowTargetCrosshair;
    public bool showTargetLine;
    bool preShowTargetLine;
    public bool chasingTarget;
    public bool isLeft;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < allPatterns.Count; i++)     //소지한 모든 패턴데이터에 자신의 정보 삽입
        {
            allPatterns[i].patternData.Init(transform,this);
        }

        showTargetCrosshair = false;
        preShowTargetCrosshair = false;
        showTargetLine = false;
        preShowTargetLine = false;
    }

    private void Start()
    {
        //보스시작연출 패턴 만들면 추가
        StartCoroutine(PatternLoop());
    }

    private void Update()
    {
        if (chasingTarget)
        {
            Rotate();
        }
            
        if (showTargetCrosshair != preShowTargetCrosshair)
        {

            preShowTargetCrosshair = showTargetCrosshair;
        }
        if (showTargetCrosshair)
        {
            targetCrosshair.position = target.position;
        }
        if (showTargetLine != preShowTargetLine)
        {

            preShowTargetLine = showTargetLine;
        }

    }

    void Rotate()
    {

        if (isLeft)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        //sprite.flipX = isleft 로 바꿔야함(스프라이트를 넣고나서)
    }

    /// <summary>
    /// 보스패턴 지속적 호출 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator PatternLoop()
    {
        while (true)
        {
            BossPattern next = GetRandomPattern();
            if (next != null)
            {
                target = next.patternData.target;
                yield return StartCoroutine(next.patternData.ExecutePattern(transform,animator));
            }
            else
            {
                Debug.LogWarning("선택 가능한 패턴 없음. 대기");//전체 영역가지는 기본패턴 추가전 임시 디버그용
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 각 패턴이 가지는 감지영역 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        if(allPatterns.Count == 0)
            return;
        for(int i = 0; i < allPatterns.Count; i++)
        {
            if (allPatterns[i] == null || allPatterns[i].patternData.attackableAreas.Count == 0)
                return;
            if (!allPatterns[i].showGizmos)
                return;
            Gizmos.color = allPatterns[i].patternData.gizmoColor;
            foreach (Rect rect in allPatterns[i].patternData.attackableAreas)
            {
                // Rect는 2D 기준: position은 좌하단 기준, size는 width/height
                Vector3 center = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, transform.position.z);
                center = transform.TransformPoint(center);
                Vector3 size = new Vector3(rect.width, rect.height, 0f);
                

                Gizmos.DrawCube(center, size);
            }
        }
    }

    public void StartNextPattern()
    {
        StartCoroutine(GetRandomPattern().patternData.ExecutePattern(transform, animator));
    }
    BossPattern GetRandomPattern()
    {
        // 가능한 패턴만 필터링
        List<BossPattern> availablePatterns = allPatterns.Where(p => p.patternData.IsAvailable()).ToList();

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
}
