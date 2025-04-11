using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossProjectileCollider : MonoBehaviour
{
    List<Player> hitPlayers = new List<Player>();
    bool ispiercing;
    Action hit;
    int multiAttackCount;
    bool _hited;
    public void colliderSet(Action hit,int multiAttackCount = 1,bool isPiercing = false)
    {
        this.multiAttackCount = multiAttackCount;
        this.ispiercing = isPiercing;
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
            if(!ispiercing && hitPlayers.Count >= multiAttackCount)
            {
                _hited = true;
                hit?.Invoke();
            }
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("GroundPlane"))
        {
            _hited = true;
            hit?.Invoke();
        }
        /*else if(쉴드 스크립트 가지고있다면)
        {
            returnToPool?.Invoke();
        }
        */
    }
}
