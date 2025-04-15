using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossProjectileCollider : MonoBehaviour
{
    List<Player> hitPlayers = new List<Player>();
    Action hit;
    int piercingAttackCount;
    bool _hited;
    LayerMask hitLayerMask;

    private void Awake()
    {
        hitLayerMask = LayerMask.GetMask("GroundPlane", "Shield");
    }
    public void colliderSet(Action hit, int piercingAttackCount = 1)
    {
        this.piercingAttackCount = piercingAttackCount;
        this.hit = hit;
        _hited = false;
        hitPlayers.Clear();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_hited)
        {
            return;
        }
        if(collision.TryGetComponent<Player>(out Player player) && !hitPlayers.Contains(player))
        {
            //player에 대미지 주는코드
            hitPlayers.Add(player);
            if(hitPlayers.Count < piercingAttackCount)
            {
                return;
            }
            _hited = true;
            hit?.Invoke();
        }
        else if(((1 << collision.gameObject.layer) & hitLayerMask) != 0 )
        {
            _hited = true;
            hit?.Invoke();
        }
    }
}
