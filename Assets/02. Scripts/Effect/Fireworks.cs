using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : MonoBehaviour
{
    [field: SerializeField] public GameObject FireworksObject { get; private set; }

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
        Instantiate(FireworksObject);
    }
}
