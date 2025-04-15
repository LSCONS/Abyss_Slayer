using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public float Hp;

    public void TakeDamage(float damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}