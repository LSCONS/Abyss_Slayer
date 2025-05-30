using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [field: SerializeField] public Slider progressBar { get; private set; }

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
    /// <param name="value"></param>
    public void SetProgressValue(float value)
    {
        progressBar.value = value;
    }

    /// <summary>
    /// 프로그래스바 값을 더해주는 메서드
    /// </summary>
    /// <param name="value"></param>
    public void AddProgressValue(float value)
    {
        progressBar.value += value;
    }
}
