using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

[Serializable]
public class Pattern
{
    public float weight;
    public BasePatternData patternData;
}
public class BossController : MonoBehaviour
{
    [SerializeField] List<Pattern> patterns = new List<Pattern>();
    private void OnDrawGizmos()
    {
        if(patterns.Count == 0)
            return;
        for(int i = 0; i < patterns.Count; i++)
        {
            if (patterns[i] == null || patterns[i].patternData.attackableAreas.Count == 0)
                return;
            Gizmos.color = patterns[i].patternData.gizmoColor;

            foreach (Rect rect in patterns[i].patternData.attackableAreas)
            {
                // Rect는 2D 기준: position은 좌하단 기준, size는 width/height
                Vector3 center = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, transform.position.z);
                Vector3 size = new Vector3(rect.width, rect.height, 0f);

                Gizmos.DrawWireCube(center, size);
            }
        }
        
    }

    /*
    public BossPattern GetRandomPattern(List<BossPattern> allPatterns)
    {
        // Step 1: 가능한 패턴만 필터링
        List<BossPattern> availablePatterns = allPatterns
            .Where(p => p.IsAvailable())
            .ToList();

        if (availablePatterns.Count == 0)
            return null;

        // Step 2: 가중치 총합 계산
        float totalWeight = availablePatterns.Sum(p => p.weight);

        // Step 3: 랜덤 값 생성
        float rand = Random.Range(0f, totalWeight);

        // Step 4: 해당 구간의 패턴 선택
        float cumulative = 0f;
        foreach (var pattern in availablePatterns)
        {
            cumulative += pattern.weight;
            if (rand <= cumulative)
            {
                return pattern;
            }
        }

        return availablePatterns.Last(); // 예외 방지용 (총합이 float 연산으로 어긋날 경우)
    }
    */
}
