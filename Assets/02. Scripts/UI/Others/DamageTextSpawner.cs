using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageTextSpawner
{
    // 현재 살아있는 텍스트 큐 (선입선출)
    public static Queue<UIDamageText> activeTexts = new();

    private static int maxTextCount = 10;
    private static float textYOffsetStep = 0.5f;
    private static float baseOffsetY = 1f;

    /// <summary>
    /// 데미지 텍스트를 월드 좌표에 생성해줌
    /// </summary>
    /// <param name="damage">표시할 데미지 값</param>
    /// <param name="worldPosition">어디다가 보여줄지 좌표</param>
    public static void Show(int damage, Vector3 worldPosition)
    {
        // 큐에서 비활성화된 텍스트 없애기
        while (activeTexts.Count > 0 && !activeTexts.Peek().gameObject.activeSelf)
        {
            activeTexts.Dequeue();
        }

        // 현재 줄 중 비어있는 index 찾기
        int indexToUse = -1;
        for (int i = 0; i < maxTextCount; i++)
        {
            bool isOccupied = false;

            foreach (var txt in activeTexts)
            {
                if (txt.gameObject.activeSelf && txt.OffsetIndex == i)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                indexToUse = i;
                break;
            }
        }

        // 못 찾았으면 가장 오래된 텍스트 제거 (덮어쓰기)
        if (indexToUse == -1)
        {
            var old = activeTexts.Dequeue();
            indexToUse = old.OffsetIndex;
            old.Rpc_ReturnToPool();
        }


        //// 현재 오프셋 위치 계산
        //int index = activeTexts.Count % maxTextCount;                               // 살아있는 텍스트 수 기반으로 인덱스 계산
        float offsetY = baseOffsetY + (textYOffsetStep * indexToUse);                    // 거기다가 오프셋 계산해서 추가
        Vector3 spawnPosition = worldPosition + new Vector3(0f, offsetY, 0f);       

        // 중첩되면 거기에 살아있는 텍스트 있으면 강제로 풀에 리턴해버리기(텍스트 없애기)
        if(activeTexts.Count >= maxTextCount)
        {
            var old = activeTexts.Dequeue();
            old.Rpc_ReturnToPool(); // 여기서 강제로 풀로 리턴해버리기
        }

        // 풀에서 꺼내기
        var text = PoolManager.Instance.Get<UIDamageText>(UIManager.Instance.canvas.transform);
        text.OffsetIndex = indexToUse;  // 인덱스에 저장하기
        text.Show(damage.ToString(), spawnPosition);

        // 큐에 추가
        activeTexts.Enqueue(text);
    }
}
