using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileNormal : MonoBehaviour
{
    Vector3 direction;
    float speed;
    float damage;
    public void Init(Vector3 direction, float speed, float damange)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
