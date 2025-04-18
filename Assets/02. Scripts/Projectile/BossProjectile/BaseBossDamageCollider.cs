using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossDamageCollider : MonoBehaviour
{
    HashSet<Player> hitPlayers = new HashSet<Player>();

    int _damage;
    Action _destroy;

    int piercingAttackCount;
    bool _destroyed;
    LayerMask hitLayerMask;

    private void Awake()
    {
        hitLayerMask = LayerData.GroundPlaneLayerMask | LayerData.ShieldLayerMask;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit">플레이어 맞췄을때(데미지주는 액션)</param>
    /// <param name="destroy">지정된 수 만큼 데미지 준 뒤 없어지는 액션</param>
    /// <param name="piercingAttackCount">관통 횟수(int.maxvalue 면 완전 관통)</param>
    public void Init(int damage,Action destroy, int piercingAttackCount = 1)
    {
        this.piercingAttackCount = piercingAttackCount;
        _damage = damage;
        _destroy = destroy;
        _destroyed = false;
        hitPlayers.Clear();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_destroyed)
        {
            return;
        }

        if(collision.transform.parent?.TryGetComponent<Player>(out Player player) != null && !(player.playerStateMachine.IsDash) && hitPlayers.Add(player))
        {
            if (hitPlayers.Count < piercingAttackCount)
            {
                return;
            }
            player.ChangePlayerHP(-_damage);            //데미지 입힘
            _destroyed = true;
            _destroy?.Invoke();
        }

        if(((1 << collision.gameObject.layer) & hitLayerMask) != 0 )
        {
            _destroyed = true;
            _destroy?.Invoke();
            return;
        }
    }
}
