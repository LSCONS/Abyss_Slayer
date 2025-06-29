using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSlash : BasePoolable
{
    [SerializeField] Animator _animator;
    [SerializeField] List<Collider2D> colliders;
    [SerializeField] EAudioClip sound;
    int _damage;
    List<Player> _hitPlayers = new List<Player>();


    public override void Rpc_Init()
    {
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Init(Vector3 position, bool isLeft, int damage, int hash, float speed, float scaleX = 9, float scaleY = 7)
    {
        gameObject.SetActive(true);
        for(int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }
        _animator.SetFloat(AnimationHash.SpeedParameterHash, speed);

        transform.position = position;
        transform.rotation = Quaternion.Euler(0, isLeft ? 0 : 180, 0);
        _damage = damage;
        _animator.SetTrigger(hash);
        transform.localScale = new Vector3(scaleX/9, scaleY/7,1);
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
            player.Rpc_Damage(_damage);
        }
    }
    public void Sound()
    {
        ManagerHub.Instance.SoundManager.PlaySFX(sound);
    }
}
