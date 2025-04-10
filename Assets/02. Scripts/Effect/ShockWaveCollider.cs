using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveCollider : MonoBehaviour
{
    [SerializeField] ShockWave _shockWave; //프리펩의 부모 오브젝트에서 드래그(인스펙터) 할당됨
    List<Player> _hitPlayers = new List<Player>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

}
