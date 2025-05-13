using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageTextSpawner
{
    private static Transform cachedCanvasTransform;     // 캔버스 한 번 찾아서 저장해줌 

    /// <summary>
    /// 데미지 텍스트를 월드 좌표에 생성해줌
    /// </summary>
    /// <param name="damage">표시할 데미지 값</param>
    /// <param name="worldPosition">어디다가 보여줄지 좌표</param>
    public static void Show(int damage, Vector3 worldPosition)
    {
        // 캔버스 찾아줌(한 번만)
        if (cachedCanvasTransform == null)
        {
            var canvasObj = GameObject.Find("Canvas");
            if (canvasObj != null)
                cachedCanvasTransform = canvasObj.transform;
            else
            {
                Debug.LogError("Canvas 못찾음");
                return;
            }
        }
        // 풀에서 데미지 텍스트 객체 가져옴(부모 캔버스도 지정해줌)
        var text = PoolManager.Instance.Get<UIDamageText>(cachedCanvasTransform);
        // 월드 좌표 그대로 전달해줘야됨 (UIDamageText에서 월드 투 스크린 해줄거임)
        text.Show(damage.ToString(), worldPosition);
    }
}
