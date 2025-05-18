using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    Laser laser;
    private void Awake()
    {
        laser = GetComponentInParent<Laser>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player) && !laser._players.Contains(player))
        {
            laser._players.Add(player);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            laser._players.Remove(player);
        }
    }
}
