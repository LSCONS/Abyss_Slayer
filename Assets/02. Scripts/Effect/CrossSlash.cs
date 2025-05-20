using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSlash : BasePoolable
{
    [SerializeField] Animator _animator;
    [SerializeField] List<Collider2D> colliders;
    

    int _damage;
    List<Player> _hitPlayers = new List<Player>();
    public override void Rpc_Init()
    {
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(Vector3 position, bool isLeft, int damage, int typeNum, float speed = 1, float scale = 1f)
    {
        for(int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }
        _animator.SetFloat("Speed", speed);

        transform.position = position;
        transform.rotation = Quaternion.Euler(0, isLeft ? 0 : 180, 0);
        _damage = damage;
        _animator.SetTrigger($"CrossSlash{typeNum}");
    }

    public void Damage(int i)
    {
        _hitPlayers.Clear();
        colliders[i].enabled = true;
    }
    public void DamageEnd(int i)
    {
        colliders[i].enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && !_hitPlayers.Contains(player))
        {
            _hitPlayers.Add(player);
            player.Damage(_damage);
        }
    }
}
