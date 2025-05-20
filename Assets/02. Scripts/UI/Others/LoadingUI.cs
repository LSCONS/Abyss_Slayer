using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [Header("로딩 팁 데이터")]
    [SerializeField] private LoadingTipData tipData;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tipText;

    [SerializeField] private float tipChangeInterval = 3f;

    private void Start()
    {
        if (tipData == null || tipData.tips.Count == 0)
        {
            tipText.text = "로딩 중...";
            return;
        }

        StartCoroutine(ChangeTipsRoutine());
    }

    private IEnumerator ChangeTipsRoutine()
    {
        while (true)
        {
            string tip = tipData.tips[Random.Range(0, tipData.tips.Count)];
            tipText.text = tip;
            yield return new WaitForSeconds(tipChangeInterval);
        }
    }
}
