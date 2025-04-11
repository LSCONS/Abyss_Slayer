using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : BasePoolable
{
    [SerializeField] private int damage = 50;           // 화살 데미지
    [SerializeField] private float arrowSpeed = 60f;    // 화살 속도
    private float maxRange;
    private Vector3 direction;
    private Vector3 initPos;

    private void Update()
    {
        // 최대 거리 도달 시 풀에 반환
        if (Vector3.Distance(initPos, transform.position) >= maxRange)
        {
            ReturnToPool();
        }

        // 화살 이동
        transform.Translate(direction * arrowSpeed * Time.deltaTime);
    }

    public override void Init()
    {
        // 호출용
    }

    public void Init(Vector3 spawnPos, Vector3 dir, float range, float speed)
    {
        transform.position = spawnPos;
        initPos = spawnPos;
        direction = dir.normalized;
        maxRange = range;
        arrowSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Boss>(out Boss boss))
        {
            //boss.TakeDamage(damage); // 데미지 전달
        }

        ReturnToPool(); // 투사체 제거
    }
}
