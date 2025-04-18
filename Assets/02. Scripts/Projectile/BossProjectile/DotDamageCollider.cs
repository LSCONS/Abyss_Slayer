using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotDamageCollider : MonoBehaviour
{
    HashSet<Player> hitPlayers = new HashSet<Player>();

    int _damage;
    float _attackRate;
    float _preDamageTime;



    private void Update()
    {
        if(Time.time - _preDamageTime >= _attackRate)
        {
            _preDamageTime = Time.time;
            foreach(Player p in hitPlayers)
            {
                p.OnDamagePlayerHP(_damage);
            }
        }
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackPerSec">1초당 데미지 부여 횟수</param>
    public void Init(int damage, float attackPerSec)
    {
        _damage = damage;
        _attackRate = 1/attackPerSec;
        hitPlayers.Clear();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            hitPlayers.Add(player);
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            hitPlayers.Remove(player);
        }
    }
}
