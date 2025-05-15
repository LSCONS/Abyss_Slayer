using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSlash : BasePoolable
{
    [SerializeField] List<Collider2D> colliders;
    int _damage;
    List<Player> players = new List<Player>();
    [SerializeField] Animator animator;
    public override void Init()
    {
    }
    public void Init(Vector3 position, int damage, bool isleft, float angle, float speed = 1)
    {
        transform.position = position;
        _damage = damage;
        transform.rotation = Quaternion.Euler(0,isleft? 180: 0, angle);
        animator.SetFloat("Speed",speed);

    }
    public void Damage()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Damage(_damage);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Player>(out Player player) && !players.Contains(player))
        {
            players.Add(player);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            players.Remove(player);
        }
    }
}
