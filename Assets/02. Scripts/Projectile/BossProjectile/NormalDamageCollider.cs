using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDamageCollider : NetworkBehaviour
{
    HashSet<Player> hitPlayers = new HashSet<Player>();

    int _damage;
    int piercingAttackCount;
    bool _destroyed;
    Action _destroy;
    LayerMask hitLayerMask;

    private void Awake()
    {
        hitLayerMask = LayerData.GroundPlaneLayerMask | LayerData.ShieldLayerMask | LayerData.GroundWallLayerMask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit">플레이어 맞췄을때(데미지주는 액션)</param>
    /// <param name="destroy">지정된 수 만큼 데미지 주거나 벽/바닥 충돌시 파괴되는 액션 삽입, 비파괴공격시 null</param>
    /// <param name="piercingAttackCount">관통 횟수(int.maxvalue 면 완전 관통)</param>
    public void Init(int damage, Action destroy, int piercingAttackCount = 1)
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

        if (collision.transform.TryGetComponent<Player>(out Player player) &&
            !(player.PlayerStateMachine.IsDash) &&
            hitPlayers.Add(player))
        {
            player.Rpc_Damage(_damage);            //데미지 입힘
            if (hitPlayers.Count < piercingAttackCount)
            {
                return;
            }

            _destroyed = true;
            _destroy?.Invoke();
        }

        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            _destroyed = true;
            _destroy?.Invoke();
            return;
        }
    }

    public void ClearHitList()
    {
        hitPlayers.Clear();
    }
}
