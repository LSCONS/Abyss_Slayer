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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < allPatterns.Count; i++)
        {
            allPatterns[i].patternData.Init(transform);
        }
    }

    private void Start()
    {
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (true)
        {
            BossPattern next = GetRandomPattern();
            if (next != null)
            {
                yield return StartCoroutine(next.patternData.ExecutePattern(transform,animator));
            }
            else
            {
                Debug.LogWarning("선택 가능한 패턴 없음. 대기");
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
        // Step 1: 가능한 패턴만 필터링
        List<BossPattern> availablePatterns = allPatterns.Where(p => p.patternData.IsAvailable()).ToList();

        if (availablePatterns.Count == 0)
            return null;

        // Step 2: 가중치 총합 계산
        float totalWeight = availablePatterns.Sum(p => p.weight);

        // Step 3: 랜덤 값 생성
        float selectedValue = UnityEngine.Random.Range(0f, totalWeight);

        // Step 4: 해당 구간의 패턴 선택
        float cumulative = 0f;
        foreach (var pattern in availablePatterns)
        {
            cumulative += pattern.weight;
            if (selectedValue <= cumulative)
            {
                return pattern;
            }
        }
        return availablePatterns.Last(); // 예외 방지용 (총합이 float 연산으로 어긋날 경우)
    }
}
