using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : MonoBehaviour
{
    [SerializeField] private GameObject fireworks;
    private void Start()
    {
        GameFlowManager.Instance.fireworks = this;
    }


    /// <summary>
    /// 불꽃놀이 시작하기
    /// </summary>
    public void StartFireworks()
    {
        StartCoroutine(DelayFireworks());
    }

    private IEnumerator DelayFireworks()
    {
        yield return new WaitForSeconds(5);
        fireworks.gameObject.SetActive(true);
    }
}
