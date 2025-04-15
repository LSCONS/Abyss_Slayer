using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void OnValidate()
    {
        if (progressBar == null)
        {
            progressBar = transform.GetGameObjectSameNameDFS("ProgressBar").GetComponent<Slider>();
        }
    }
    private void Awake()
    {
        if (progressBar == null)
        {
            if (transform.Find("ProgressBar") != null)
            {
                progressBar = transform.GetGameObjectSameNameDFS("ProgressBar").GetComponent<Slider>();
            }
        }
    }


    /// <summary>
    /// 프로그레스바 값 설정해주는 메서드
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(float progress)
    {
        progressBar.value = progress;
    }
}
